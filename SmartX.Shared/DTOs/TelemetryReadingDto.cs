using SmartX.Shared.Enums;

namespace SmartX.Shared.DTOs;

public class TelemetryReadingDto
{
    public Guid Id { get; set; }

    public Guid SensorDeviceId { get; set; }

    public DateTime TimestampUtc { get; set; }

    public TelemetryDataType DataType { get; set; }

    public string MetricName { get; set; } = string.Empty;

    public double NumericValue { get; set; }

    public bool BooleanValue { get; set; }

    public string? StringValue { get; set; }

    public string Unit { get; set; } = string.Empty;

    public bool IsAnomaly { get; set; }

    public AlertSeverity? AlertSeverity { get; set; }
}