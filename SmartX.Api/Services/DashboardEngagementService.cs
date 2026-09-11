using SmartX.Shared.Models;

namespace SmartX.Api.Services;

public sealed class DashboardEngagementService
{
    private readonly object _lock = new();

    private readonly List<UserIssue> _issues = new();

    // =========================================================
    // ENGAGEMENT SNAPSHOT
    // =========================================================

    public DashboardEngagementSnapshot GetEngagementSnapshot()
    {
        lock (_lock)
        {
            var issues = _issues
                .Select(CloneIssue)
                .ToList();

            var openIssues = issues
                .Where(IsOpen)
                .ToList();

            var criticalCount = openIssues.Count(
                x => x.Severity.Equals(
                    "Critical",
                    StringComparison.OrdinalIgnoreCase));

            var warningCount = openIssues.Count(
                x => x.Severity.Equals(
                    "Warning",
                    StringComparison.OrdinalIgnoreCase));

            var infoCount = openIssues.Count(
                x => x.Severity.Equals(
                    "Info",
                    StringComparison.OrdinalIgnoreCase));

            var priorityIssues = openIssues
                .OrderByDescending(GetPriorityScore)
                .ThenByDescending(x => x.ReportedAtUtc)
                .Take(10)
                .ToList()
                .AsReadOnly();

            var recentActivity = issues
                .OrderByDescending(x => x.ReportedAtUtc)
                .Take(10)
                .ToList()
                .AsReadOnly();

            return new DashboardEngagementSnapshot
            {
                TotalIssues = issues.Count,

                OpenIssues = openIssues.Count,

                ResolvedIssues =
                    issues.Count - openIssues.Count,

                CriticalOpenIssues =
                    criticalCount,

                WarningOpenIssues =
                    warningCount,

                InformationalOpenIssues =
                    infoCount,

                AttentionLevel =
                    DetermineAttentionLevel(
                        criticalCount,
                        warningCount,
                        infoCount),

                PriorityIssues =
                    priorityIssues,

                RecentActivity =
                    recentActivity,

                GeneratedAtUtc =
                    DateTime.UtcNow
            };
        }
    }

    // =========================================================
    // ISSUE LIST
    // =========================================================

    public IReadOnlyList<UserIssue> GetIssues()
    {
        lock (_lock)
        {
            return _issues
                .Select(CloneIssue)
                .OrderByDescending(
                    x => x.ReportedAtUtc)
                .ToList()
                .AsReadOnly();
        }
    }

    // =========================================================
    // ADD ISSUE
    // =========================================================

    public UserIssue AddIssue(
        string title,
        string description,
        string severity)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Issue title is required.",
                nameof(title));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "Issue description is required.",
                nameof(description));
        }

        var normalisedSeverity =
            NormaliseSeverity(severity);

        var issue = new UserIssue
        {
            Id = Guid.NewGuid(),

            Title =
                title.Trim(),

            Description =
                description.Trim(),

            Severity =
                normalisedSeverity,

            Status =
                "Open",

            ReportedAtUtc =
                DateTime.UtcNow
        };

        lock (_lock)
        {
            _issues.Add(issue);

            return CloneIssue(issue);
        }
    }

    // =========================================================
    // RESOLVE ISSUE
    // =========================================================

    public bool ResolveIssue(Guid issueId)
    {
        if (issueId == Guid.Empty)
        {
            return false;
        }

        lock (_lock)
        {
            var issue =
                _issues.FirstOrDefault(
                    x => x.Id == issueId);

            if (issue is null)
            {
                return false;
            }

            if (issue.Status.Equals(
                    "Resolved",
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            issue.Status = "Resolved";

            return true;
        }
    }

    // =========================================================
    // OPEN ISSUE COUNT
    // =========================================================

    public int GetOpenIssueCount()
    {
        lock (_lock)
        {
            return _issues.Count(IsOpen);
        }
    }

    // =========================================================
    // PRIORITY CALCULATION
    // =========================================================

    private static int GetPriorityScore(
        UserIssue issue)
    {
        return issue.Severity
            .Trim()
            .ToLowerInvariant() switch
        {
            "critical" => 300,
            "warning" => 200,
            "info" => 100,
            _ => 50
        };
    }

    // =========================================================
    // ATTENTION LEVEL
    // =========================================================

    private static string DetermineAttentionLevel(
        int criticalCount,
        int warningCount,
        int infoCount)
    {
        if (criticalCount > 0)
        {
            return "Critical Attention Required";
        }

        if (warningCount > 0)
        {
            return "Attention Required";
        }

        if (infoCount > 0)
        {
            return "Monitoring";
        }

        return "All Clear";
    }

    // =========================================================
    // STATUS CHECK
    // =========================================================

    private static bool IsOpen(
        UserIssue issue)
    {
        return issue.Status.Equals(
            "Open",
            StringComparison.OrdinalIgnoreCase);
    }

    // =========================================================
    // SEVERITY NORMALISATION
    // =========================================================

    private static string NormaliseSeverity(
        string? severity)
    {
        if (string.IsNullOrWhiteSpace(severity))
        {
            return "Info";
        }

        return severity
            .Trim()
            .ToLowerInvariant() switch
        {
            "info" =>
                "Info",

            "warning" =>
                "Warning",

            "critical" =>
                "Critical",

            _ => throw new ArgumentException(
                "Severity must be Info, Warning, or Critical.",
                nameof(severity))
        };
    }

    // =========================================================
    // SAFE ISSUE COPY
    // =========================================================

    private static UserIssue CloneIssue(
        UserIssue source)
    {
        return new UserIssue
        {
            Id =
                source.Id,

            Title =
                source.Title,

            Description =
                source.Description,

            Severity =
                source.Severity,

            Status =
                source.Status,

            ReportedAtUtc =
                source.ReportedAtUtc
        };
    }
}