using SmartX.Shared.Enums;

namespace SmartX.Shared.Models;

public class TelemetryAnomaly
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SensorDeviceId { get; set; }

    public Guid? TelemetryReadingId { get; set; }

    public string MetricName { get; set; } = string.Empty;

    public double ObservedValue { get; set; }

    public double? ExpectedMinimum { get; set; }

    public double? ExpectedMaximum { get; set; }

    public AlertSeverity Severity { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime DetectedAtUtc { get; set; } = DateTime.UtcNow;

    public bool Resolved { get; set; }

    public DateTime? ResolvedAtUtc { get; set; }
}