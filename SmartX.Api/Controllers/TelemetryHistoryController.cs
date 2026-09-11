using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/telemetry-history")]
public class TelemetryHistoryController : ControllerBase
{
    private readonly TelemetryHistoryService _historyService;

    public TelemetryHistoryController(TelemetryHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet("{sensorId:guid}")]
    public ActionResult<IReadOnlyList<double>> Get(Guid sensorId)
    {
        return Ok(_historyService.GetReadings(sensorId));
    }

    [HttpGet("{sensorId:guid}/matrix")]
    public ActionResult<double[,]> GetMatrix(
        Guid sensorId,
        [FromQuery] int rows = 5,
        [FromQuery] int columns = 5)
    {
        if (rows <= 0 || columns <= 0)
        {
            return BadRequest("Rows and columns must be greater than zero.");
        }

        return Ok(_historyService.GetMatrix(sensorId, rows, columns));
    }
}