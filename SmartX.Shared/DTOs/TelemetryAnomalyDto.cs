using SmartX.Shared.Enums;

namespace SmartX.Shared.DTOs;

public class TelemetryAnomalyDto
{
    public Guid Id { get; set; }

    public Guid SensorDeviceId { get; set; }

    public Guid? TelemetryReadingId { get; set; }

    public string MetricName { get; set; } = string.Empty;

    public double ObservedValue { get; set; }

    public double? ExpectedMinimum { get; set; }

    public double? ExpectedMaximum { get; set; }

    public AlertSeverity Severity { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime DetectedAtUtc { get; set; }

    public bool Resolved { get; set; }

    public DateTime? ResolvedAtUtc { get; set; }
}