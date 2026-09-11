namespace SmartX.Shared.DTOs;

public class TelemetryStatisticsDto
{
    public string MetricName { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public int SampleCount { get; set; }

    public double Minimum { get; set; }

    public double Maximum { get; set; }

    public double Average { get; set; }

    public double LatestValue { get; set; }

    public double PreviousValue { get; set; }

    public double Delta { get; set; }

    public double DeltaPercentage { get; set; }

    public int AnomalyCount { get; set; }

    public DateTime? LatestTimestampUtc { get; set; }
}