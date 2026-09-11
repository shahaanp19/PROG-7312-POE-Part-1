using SmartX.Shared.Enums;

namespace SmartX.Shared.Models;

public class TelemetryValidationResult
{
    public bool IsValid { get; set; }

    public bool IsAnomaly { get; set; }

    public AlertSeverity? Severity { get; set; }

    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public string? Message { get; set; }

    public DateTime ValidatedAtUtc { get; set; } = DateTime.UtcNow;
}