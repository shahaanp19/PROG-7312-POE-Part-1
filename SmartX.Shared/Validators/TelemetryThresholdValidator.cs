using SmartX.Shared.Models;

namespace SmartX.Shared.Validators;

public class TelemetryThresholdValidator
{
    public bool IsWithinExpectedRange(
        TelemetryMetric metric,
        double value)
    {
        return value >= metric.MinimumExpectedValue &&
               value <= metric.MaximumExpectedValue;
    }

    public bool IsWarning(
        TelemetryMetric metric,
        double value)
    {
        return value >= metric.WarningThreshold;
    }

    public bool IsCritical(
        TelemetryMetric metric,
        double value)
    {
        return value >= metric.CriticalThreshold;
    }

    public bool IsAnomaly(
        TelemetryMetric metric,
        double value)
    {
        return !IsWithinExpectedRange(metric, value);
    }
}