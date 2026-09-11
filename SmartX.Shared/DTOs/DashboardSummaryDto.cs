using SmartX.Shared.Enums;

namespace SmartX.Shared.DTOs;

public class DashboardSummaryDto
{
    public int TotalDevices { get; set; }

    public int OnlineDevices { get; set; }

    public int OfflineDevices { get; set; }

    public int WarningDevices { get; set; }

    public int CriticalDevices { get; set; }

    public int MaintenanceDevices { get; set; }

    public int ActiveAlerts { get; set; }

    public int CriticalAlerts { get; set; }

    public int TelemetryReadingsReceived { get; set; }

    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;

    public Dictionary<SensorCategory, int> DevicesByCategory { get; set; } = [];
}