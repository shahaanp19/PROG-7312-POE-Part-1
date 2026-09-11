namespace SmartX.Shared.Models;

public sealed class TelemetryPacket<T>
{
    public Guid PacketId { get; init; } = Guid.NewGuid();

    public Guid SensorDeviceId { get; init; }

    public string MetricName { get; init; } = string.Empty;

    public T Value { get; init; } = default!;

    public string Unit { get; init; } = string.Empty;

    public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;

    public TelemetryPacket(
        Guid sensorDeviceId,
        string metricName,
        T value,
        string unit)
    {
        SensorDeviceId = sensorDeviceId;
        MetricName = metricName;
        Value = value;
        Unit = unit;
    }
}