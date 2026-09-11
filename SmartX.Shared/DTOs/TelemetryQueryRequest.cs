using System.ComponentModel.DataAnnotations;

namespace SmartX.Shared.DTOs;

public class TelemetryQueryRequest
{
    [Required]
    public Guid SensorDeviceId { get; set; }

    public DateTime? FromUtc { get; set; }

    public DateTime? ToUtc { get; set; }

    [Range(1, 5000)]
    public int Limit { get; set; } = 100;

    public bool IncludeAnomaliesOnly { get; set; }

    public string? MetricName { get; set; }
}