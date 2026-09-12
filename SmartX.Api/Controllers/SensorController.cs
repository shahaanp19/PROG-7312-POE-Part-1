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


//References
//tdykstra (2024). Create web APIs with ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//wadepickett (2025). Tutorial: Create a controller-based web API with ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0&tabs=visual-studio [Accessed 12 Sept. 2026].
//tdykstra (2025). Model validation in ASP.NET Core MVC. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//tdykstra (2024). Dependency injection in ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].