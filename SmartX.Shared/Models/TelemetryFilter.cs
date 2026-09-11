using SmartX.Shared.Enums;

namespace SmartX.Shared.Models;

public class TelemetryFilter
{
    public Guid? SensorDeviceId { get; set; }

    public string? MetricName { get; set; }

    public TelemetryDataType? DataType { get; set; }

    public DateTime? FromUtc { get; set; }

    public DateTime? ToUtc { get; set; }

    public double? MinimumValue { get; set; }

    public double? MaximumValue { get; set; }

    public bool AnomaliesOnly { get; set; }

    public AlertSeverity? MinimumSeverity { get; set; }

    public int Limit { get; set; } = 100;
}