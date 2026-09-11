namespace SmartX.Shared.Models;

public sealed class UserIssue
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Severity { get; set; } = "Info";

    public string Status { get; set; } = "Open";

    public DateTime ReportedAtUtc { get; set; } =
        DateTime.UtcNow;
}