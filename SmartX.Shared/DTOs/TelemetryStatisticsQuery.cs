using System.ComponentModel.DataAnnotations;

namespace SmartX.Shared.DTOs;

public class TelemetryStatisticsQuery
{
    [Required]
    public Guid SensorDeviceId { get; set; }

    public DateTime? FromUtc { get; set; }

    public DateTime? ToUtc { get; set; }

    public List<string> MetricNames { get; set; } = [];

    [Range(1, 5000)]
    public int MaximumSamplesPerMetric { get; set; } = 1000;
}