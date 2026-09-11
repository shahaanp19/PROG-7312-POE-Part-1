using SmartX.Shared.Models;

namespace SmartX.Shared.DTOs;

public class TelemetryMetricDto
{
    public Guid Id { get; set; }

    public Guid SensorDeviceId { get; set; }

    public string MetricName { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public double MinimumExpectedValue { get; set; }

    public double MaximumExpectedValue { get; set; }

    public double WarningThreshold { get; set; }

    public double CriticalThreshold { get; set; }

    public bool IsEnabled { get; set; }
}