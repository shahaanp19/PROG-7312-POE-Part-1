namespace SmartX.Api.Services;

public class SensorRegistryService
{
    private readonly Dictionary<Guid, object> _sensors = new();
    private readonly object _syncLock = new();

    public IReadOnlyCollection<object> GetAll()
    {
        lock (_syncLock)
        {
            return _sensors.Values.ToList().AsReadOnly();
        }
    }

    public bool Contains(Guid sensorId)
    {
        if (sensorId == Guid.Empty)
        {
            return false;
        }

        lock (_syncLock)
        {
            return _sensors.ContainsKey(sensorId);
        }
    }

    public bool Register(Guid sensorId, object sensor)
    {
        ArgumentNullException.ThrowIfNull(sensor);

        if (sensorId == Guid.Empty)
        {
            return false;
        }

        lock (_syncLock)
        {
            if (_sensors.ContainsKey(sensorId))
            {
                return false;
            }

            _sensors[sensorId] = sensor;
            return true;
        }
    }

    public bool Remove(Guid sensorId)
    {
        if (sensorId == Guid.Empty)
        {
            return false;
        }

        lock (_syncLock)
        {
            return _sensors.Remove(sensorId);
        }
    }

    public object? Get(Guid sensorId)
    {
        if (sensorId == Guid.Empty)
        {
            return null;
        }

        lock (_syncLock)
        {
            return _sensors.TryGetValue(sensorId, out var sensor)
                ? sensor
                : null;
        }
    }
}