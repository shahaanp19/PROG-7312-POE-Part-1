namespace SmartX.Api.Services;

public class SensorRegistryService
{
    private readonly Dictionary<Guid, object> _sensors = new();

    public IReadOnlyCollection<object> GetAll()
    {
        return _sensors.Values.ToList().AsReadOnly();
    }

    public bool Contains(Guid sensorId)
    {
        return _sensors.ContainsKey(sensorId);
    }

    public bool Register(Guid sensorId, object sensor)
    {
        ArgumentNullException.ThrowIfNull(sensor);

        if (_sensors.ContainsKey(sensorId))
        {
            return false;
        }

        _sensors[sensorId] = sensor;
        return true;
    }

    public bool Remove(Guid sensorId)
    {
        return _sensors.Remove(sensorId);
    }

    public object? Get(Guid sensorId)
    {
        return _sensors.TryGetValue(sensorId, out var sensor)
            ? sensor
            : null;
    }
}