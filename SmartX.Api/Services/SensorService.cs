using SmartX.Shared.DTOs;

namespace SmartX.Api.Services;

public class SensorService
{
    private readonly Dictionary<Guid, SensorRegistrationResponse> _sensors = new();

    public SensorRegistrationResponse Register(SensorRegistrationRequest request)
    {
        var sensor = new SensorRegistrationResponse
        {
            SensorId = Guid.NewGuid(),
            Name = request.Name,
            SensorType = request.SensorType,
            Location = request.Location,
            Description = request.Description,
            IsActive = request.IsActive,
            RegisteredAtUtc = DateTime.UtcNow
        };

        _sensors[sensor.SensorId] = sensor;

        return sensor;
    }

    public IReadOnlyCollection<SensorRegistrationResponse> GetAll()
    {
        return _sensors.Values.ToList().AsReadOnly();
    }

    public SensorRegistrationResponse? Get(Guid sensorId)
    {
        return _sensors.TryGetValue(sensorId, out var sensor)
            ? sensor
            : null;
    }

    public bool Remove(Guid sensorId)
    {
        return _sensors.Remove(sensorId);
    }
}