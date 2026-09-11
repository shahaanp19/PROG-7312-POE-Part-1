using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/sensors")]
public class SensorController : ControllerBase
{
    private readonly SensorRegistryService _registry;

    public SensorController(SensorRegistryService registry)
    {
        _registry = registry;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_registry.GetAll());
    }

    [HttpGet("{sensorId:guid}")]
    public IActionResult Get(Guid sensorId)
    {
        var sensor = _registry.Get(sensorId);

        return sensor is null
            ? NotFound()
            : Ok(sensor);
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "Healthy",
            service = "Sensor API",
            timestampUtc = DateTime.UtcNow
        });
    }

    [HttpDelete("{sensorId:guid}")]
    public IActionResult Remove(Guid sensorId)
    {
        return _registry.Remove(sensorId)
            ? NoContent()
            : NotFound();
    }
}