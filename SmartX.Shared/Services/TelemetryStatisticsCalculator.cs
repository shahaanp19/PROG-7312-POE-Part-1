using SmartX.Shared.DataStructures;
using SmartX.Shared.DTOs;

namespace SmartX.Shared.Services;

public class TelemetryStatisticsCalculator
{
    public TelemetryStatisticsDto Calculate(
        string metricName,
        string unit,
        IReadOnlyList<TelemetryValue<double>> values)
    {
        if (values is null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        if (values.Count == 0)
        {
            return new TelemetryStatisticsDto
            {
                MetricName = metricName,
                Unit = unit
            };
        }

        double minimum = values[0].Value;
        double maximum = values[0].Value;
        double sum = 0;

        for (int i = 0; i < values.Count; i++)
        {
            double value = values[i].Value;

            if (value < minimum)
            {
                minimum = value;
            }

            if (value > maximum)
            {
                maximum = value;
            }

            sum += value;
        }

        double latestValue = values[^1].Value;
        double previousValue = values.Count > 1
            ? values[^2].Value
            : latestValue;

        double delta = latestValue - previousValue;

        double deltaPercentage = previousValue == 0
            ? 0
            : (delta / Math.Abs(previousValue)) * 100;

        return new TelemetryStatisticsDto
        {
            MetricName = metricName,
            Unit = unit,
            SampleCount = values.Count,
            Minimum = minimum,
            Maximum = maximum,
            Average = sum / values.Count,
            LatestValue = latestValue,
            PreviousValue = previousValue,
            Delta = delta,
            DeltaPercentage = deltaPercentage
        };
    }
}