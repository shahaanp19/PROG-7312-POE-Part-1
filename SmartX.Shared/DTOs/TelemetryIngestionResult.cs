namespace SmartX.Shared.DTOs;

public class TelemetryIngestionResult
{
    public bool Success { get; set; }

    public Guid? PacketId { get; set; }

    public Guid SensorDeviceId { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool IsAnomaly { get; set; }

    public double ProcessingTimeMilliseconds { get; set; }

    public DateTime ProcessedAtUtc { get; set; } = DateTime.UtcNow;
}