namespace SmartX.Shared.Models;

public sealed class DashboardEngagementSnapshot
{
    public int TotalIssues { get; init; }

    public int OpenIssues { get; init; }

    public int ResolvedIssues { get; init; }

    public int CriticalOpenIssues { get; init; }

    public int WarningOpenIssues { get; init; }

    public int InformationalOpenIssues { get; init; }

    public string AttentionLevel { get; init; } =
        "All Clear";

    public IReadOnlyList<UserIssue> PriorityIssues { get; init; } =
        Array.Empty<UserIssue>();

    public IReadOnlyList<UserIssue> RecentActivity { get; init; } =
        Array.Empty<UserIssue>();

    public DateTime GeneratedAtUtc { get; init; }
}