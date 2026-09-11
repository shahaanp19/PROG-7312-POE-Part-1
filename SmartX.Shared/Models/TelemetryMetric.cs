using System.ComponentModel.DataAnnotations;

namespace SmartX.Shared.Models;

public class TelemetryMetric
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SensorDeviceId { get; set; }

    [Required]
    [MaxLength(100)]
    public string MetricName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Unit { get; set; } = string.Empty;

    public double MinimumExpectedValue { get; set; }

    public double MaximumExpectedValue { get; set; }

    public double WarningThreshold { get; set; }

    public double CriticalThreshold { get; set; }

    public bool IsEnabled { get; set; } = true;
}