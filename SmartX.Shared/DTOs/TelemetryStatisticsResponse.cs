namespace SmartX.Shared.DTOs;

public class TelemetryStatisticsResponse
{
    public Guid SensorDeviceId { get; set; }

    public string DeviceName { get; set; } = string.Empty;

    public DateTime FromUtc { get; set; }

    public DateTime ToUtc { get; set; }

    public int TotalReadings { get; set; }

    public int TotalAnomalies { get; set; }

    public List<TelemetryStatisticsDto> Metrics { get; set; } = [];

    public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;
}