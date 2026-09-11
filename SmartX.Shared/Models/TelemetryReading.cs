using System.ComponentModel.DataAnnotations;
using SmartX.Shared.Enums;

namespace SmartX.Shared.Models;

public class TelemetryReading
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SensorDeviceId { get; set; }

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public TelemetryDataType DataType { get; set; }

    [Required]
    [MaxLength(100)]
    public string MetricName { get; set; } = string.Empty;

    public double NumericValue { get; set; }

    public bool BooleanValue { get; set; }

    [MaxLength(500)]
    public string? StringValue { get; set; }

    [MaxLength(50)]
    public string Unit { get; set; } = string.Empty;

    public bool IsAnomaly { get; set; }

    public AlertSeverity? AlertSeverity { get; set; }
}