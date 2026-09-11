using SmartX.Shared.Enums;

namespace SmartX.Shared.DTOs;

public class TelemetryPacketDto<T>
{
    public Guid SensorDeviceId { get; set; }

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public string MetricName { get; set; } = string.Empty;

    public T Value { get; set; } = default!;

    public string Unit { get; set; } = string.Empty;

    public TelemetryDataType DataType { get; set; }
}