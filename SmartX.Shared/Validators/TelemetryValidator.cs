using SmartX.Shared.Enums;
using SmartX.Shared.Models;

namespace SmartX.Shared.Validators;

public class TelemetryValidator
{
    private readonly TelemetryThresholdValidator _thresholdValidator;

    public TelemetryValidator(
        TelemetryThresholdValidator thresholdValidator)
    {
        _thresholdValidator =
            thresholdValidator ??
            throw new ArgumentNullException(
                nameof(thresholdValidator));
    }

    public TelemetryValidationResult Validate(
        TelemetryMetric metric,
        double value)
    {
        ArgumentNullException.ThrowIfNull(metric);

        var result = new TelemetryValidationResult
        {
            IsValid = true,
            IsAnomaly = false,
            Severity = AlertSeverity.Info
        };

        if (!metric.IsEnabled)
        {
            result.IsValid = false;
            result.Errors.Add(
                "Telemetry metric validation is disabled.");
            result.Message =
                "Metric validation is disabled.";

            return result;
        }

        if (double.IsNaN(value) ||
            double.IsInfinity(value))
        {
            result.IsValid = false;
            result.IsAnomaly = true;
            result.Severity = AlertSeverity.Critical;

            result.Errors.Add(
                "Telemetry value must be a finite number.");

            result.Message =
                "Invalid numeric telemetry value.";

            return result;
        }

        if (!_thresholdValidator.HasValidThresholdConfiguration(metric))
        {
            result.IsValid = false;
            result.IsAnomaly = true;
            result.Severity = AlertSeverity.Critical;

            result.Errors.Add(
                "Telemetry threshold configuration is invalid.");

            result.Message =
                "Invalid telemetry threshold configuration.";

            return result;
        }

        var outsideExpectedRange =
            !_thresholdValidator.IsWithinExpectedRange(
                metric,
                value);

        if (outsideExpectedRange)
        {
            result.IsAnomaly = true;

            result.Severity =
                DetermineSeverity(metric, value);

            result.Warnings.Add(
                $"Telemetry value {value} is outside the expected " +
                $"range of {metric.MinimumExpectedValue} to " +
                $"{metric.MaximumExpectedValue}.");

            result.Message =
                "Telemetry anomaly detected.";
        }

        if (_thresholdValidator.IsCritical(metric, value))
        {
            result.IsAnomaly = true;
            result.Severity = AlertSeverity.Critical;

            result.Warnings.Add(
                $"Telemetry value {value} has reached or exceeded " +
                $"the critical threshold of " +
                $"{metric.CriticalThreshold}.");

            result.Message =
                "Critical telemetry threshold exceeded.";
        }
        else if (_thresholdValidator.IsWarning(metric, value))
        {
            result.IsAnomaly = true;

            if (result.Severity != AlertSeverity.Critical)
            {
                result.Severity = AlertSeverity.Warning;
            }

            result.Warnings.Add(
                $"Telemetry value {value} has reached or exceeded " +
                $"the warning threshold of " +
                $"{metric.WarningThreshold}.");

            if (string.IsNullOrWhiteSpace(result.Message))
            {
                result.Message =
                    "Telemetry warning threshold exceeded.";
            }
        }

        return result;
    }

    private static AlertSeverity DetermineSeverity(
        TelemetryMetric metric,
        double value)
    {
        if (value >= metric.CriticalThreshold)
        {
            return AlertSeverity.Critical;
        }

        if (value >= metric.WarningThreshold)
        {
            return AlertSeverity.Warning;
        }

        return AlertSeverity.Info;
    }
}