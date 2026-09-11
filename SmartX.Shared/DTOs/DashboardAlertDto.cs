using SmartX.Shared.Enums;

namespace SmartX.Shared.DTOs;

public class DashboardAlertDto
{
    public Guid Id { get; set; }

    public Guid SensorDeviceId { get; set; }

    public string DeviceName { get; set; } = string.Empty;

    public string MetricName { get; set; } = string.Empty;

    public AlertSeverity Severity { get; set; }

    public string Message { get; set; } = string.Empty;

    public double? CurrentValue { get; set; }

    public double? PreviousValue { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public bool IsAcknowledged { get; set; }
}