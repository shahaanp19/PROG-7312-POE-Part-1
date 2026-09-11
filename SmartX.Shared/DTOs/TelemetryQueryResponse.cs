namespace SmartX.Shared.DTOs;

public class TelemetryQueryResponse
{
    public int TotalCount { get; set; }

    public int ReturnedCount { get; set; }

    public int AnomalyCount { get; set; }

    public DateTime? EarliestTimestampUtc { get; set; }

    public DateTime? LatestTimestampUtc { get; set; }

    public List<TelemetryReadingDto> Readings { get; set; } = [];
}