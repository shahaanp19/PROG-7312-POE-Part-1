using SmartX.Shared.Models;

namespace SmartX.Shared.Validators;

public class TelemetryThresholdValidator
{
    public bool HasValidThresholdConfiguration(
        TelemetryMetric metric)
    {
        ArgumentNullException.ThrowIfNull(metric);

        return
            double.IsFinite(metric.MinimumExpectedValue) &&
            double.IsFinite(metric.MaximumExpectedValue) &&
            double.IsFinite(metric.WarningThreshold) &&
            double.IsFinite(metric.CriticalThreshold) &&
            metric.MinimumExpectedValue <= metric.MaximumExpectedValue &&
            metric.WarningThreshold <= metric.CriticalThreshold;
    }

    public bool IsWithinExpectedRange(
        TelemetryMetric metric,
        double value)
    {
        ArgumentNullException.ThrowIfNull(metric);

        return value >= metric.MinimumExpectedValue &&
               value <= metric.MaximumExpectedValue;
    }

    public bool IsWarning(
        TelemetryMetric metric,
        double value)
    {
        ArgumentNullException.ThrowIfNull(metric);

        return value >= metric.WarningThreshold;
    }

    public bool IsCritical(
        TelemetryMetric metric,
        double value)
    {
        ArgumentNullException.ThrowIfNull(metric);

        return value >= metric.CriticalThreshold;
    }

    public bool IsAnomaly(
        TelemetryMetric metric,
        double value)
    {
        ArgumentNullException.ThrowIfNull(metric);

        return !IsWithinExpectedRange(metric, value);
    }
}