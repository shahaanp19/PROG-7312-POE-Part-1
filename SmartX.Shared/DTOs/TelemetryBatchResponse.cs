namespace SmartX.Shared.DTOs;

public class TelemetryBatchResponse
{
    public int TotalReadings { get; set; }

    public int TotalDevices { get; set; }

    public int TotalAnomalies { get; set; }

    public DateTime FromUtc { get; set; }

    public DateTime ToUtc { get; set; }

    public List<TelemetryReadingDto> Readings { get; set; } = [];

    public List<TelemetryAnomalyDto> Anomalies { get; set; } = [];
}