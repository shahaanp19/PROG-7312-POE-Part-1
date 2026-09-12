using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;
using SmartX.Shared.DTOs;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/sensor-registration")]
public class SensorRegistrationController : ControllerBase
{
    private readonly SensorRegistryService _registry;

    private static readonly HashSet<string> AllowedSensorTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "Environmental",
            "Power Consumption",
            "Actuator"
        };

    public SensorRegistrationController(
        SensorRegistryService registry)
    {
        _registry = registry;
    }

  
    //Sensor Registration

    [HttpPost]
    public ActionResult<SensorRegistrationResponse> Register(
        [FromBody] SensorRegistrationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        // Normalise incoming values
        var name = request.Name.Trim();
        var deviceIdentifier = request.DeviceIdentifier.Trim();
        var sensorType = request.SensorType.Trim();
        var location = request.Location.Trim();
        var description = request.Description.Trim();

        // Additional protection against empty/whitespace values
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError(
                nameof(request.Name),
                "Sensor name cannot be empty.");

            return ValidationProblem(ModelState);
        }

        if (string.IsNullOrWhiteSpace(deviceIdentifier))
        {
            ModelState.AddModelError(
                nameof(request.DeviceIdentifier),
                "Device identifier cannot be empty.");

            return ValidationProblem(ModelState);
        }

        if (string.IsNullOrWhiteSpace(location))
        {
            ModelState.AddModelError(
                nameof(request.Location),
                "Deployment location cannot be empty.");

            return ValidationProblem(ModelState);
        }

        // Validate the sensor category against the supported categories
        if (!AllowedSensorTypes.Contains(sensorType))
        {
            ModelState.AddModelError(
                nameof(request.SensorType),
                "Invalid sensor category.");

            return ValidationProblem(ModelState);
        }

        // Create the registered sensor
        var sensorId = Guid.NewGuid();

        var response = new SensorRegistrationResponse
        {
            SensorId = sensorId,
            Name = name,
            DeviceIdentifier = deviceIdentifier,
            SensorType = sensorType,
            Location = location,
            Description = description,
            IsActive = request.IsActive,
            RegisteredAtUtc = DateTime.UtcNow
        };

        // Store the sensor in the registry
        if (!_registry.Register(sensorId, response))
        {
            return Conflict(new
            {
                message = "Sensor could not be registered."
            });
        }

        // Return HTTP 201 Created with a location for the new resource
        return CreatedAtAction(
            nameof(Get),
            new { sensorId },
            response);
    }

   

    [HttpGet("{sensorId:guid}")]
    public ActionResult<SensorRegistrationResponse> Get(
        Guid sensorId)
    {
        var sensor = _registry.Get(sensorId);

        if (sensor is not SensorRegistrationResponse response)
        {
            return NotFound(new
            {
                message = "Sensor was not found.",
                sensorId
            });
        }

        return Ok(response);
    }
}

//References
//tdykstra (2024). Create web APIs with ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//wadepickett (2025). Tutorial: Create a controller-based web API with ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0&tabs=visual-studio [Accessed 12 Sept. 2026].
//tdykstra (2025). Model validation in ASP.NET Core MVC. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//tdykstra (2024). Dependency injection in ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].