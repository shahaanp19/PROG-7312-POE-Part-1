using System.ComponentModel.DataAnnotations;

namespace SmartX.Shared.DTOs;

public class TelemetryBatchQueryRequest
{
    [Required]
    public List<Guid> SensorDeviceIds { get; set; } = [];

    public DateTime? FromUtc { get; set; }

    public DateTime? ToUtc { get; set; }

    [Range(1, 5000)]
    public int LimitPerDevice { get; set; } = 100;

    public bool IncludeAnomaliesOnly { get; set; }

    public string? MetricName { get; set; }
}