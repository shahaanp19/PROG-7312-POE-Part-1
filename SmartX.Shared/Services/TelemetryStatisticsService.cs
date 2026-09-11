using SmartX.Shared.DTOs;
using SmartX.Shared.Models;

namespace SmartX.Shared.Services;

public class TelemetryStatisticsService
{
    public TelemetryStatisticsDto Calculate(
        string metricName,
        string unit,
        IReadOnlyList<TelemetryReading> readings)
    {
        if (readings is null)
        {
            throw new ArgumentNullException(nameof(readings));
        }

        if (readings.Count == 0)
        {
            return new TelemetryStatisticsDto
            {
                MetricName = metricName,
                Unit = unit
            };
        }

        double minimum = double.MaxValue;
        double maximum = double.MinValue;
        double sum = 0;

        foreach (TelemetryReading reading in readings)
        {
            double value = reading.NumericValue;

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

        TelemetryReading latest = readings
            .OrderByDescending(r => r.TimestampUtc)
            .First();

        TelemetryReading? previous = readings
            .Where(r => r.Id != latest.Id)
            .OrderByDescending(r => r.TimestampUtc)
            .FirstOrDefault();

        double latestValue = latest.NumericValue;
        double previousValue = previous?.NumericValue ?? latestValue;
        double delta = latestValue - previousValue;

        double deltaPercentage = previousValue == 0
            ? 0
            : (delta / Math.Abs(previousValue)) * 100;

        return new TelemetryStatisticsDto
        {
            MetricName = metricName,
            Unit = unit,
            SampleCount = readings.Count,
            Minimum = minimum,
            Maximum = maximum,
            Average = sum / readings.Count,
            LatestValue = latestValue,
            PreviousValue = previousValue,
            Delta = delta,
            DeltaPercentage = deltaPercentage,
            AnomalyCount = readings.Count(r => r.IsAnomaly),
            LatestTimestampUtc = latest.TimestampUtc
        };
    }
}