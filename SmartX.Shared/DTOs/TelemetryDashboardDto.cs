using SmartX.Shared.Enums;

namespace SmartX.Shared.DTOs;

public class TelemetryDashboardDto
{
    public Guid SensorDeviceId { get; set; }

    public string DeviceName { get; set; } = string.Empty;

    public DeviceStatus DeviceStatus { get; set; }

    public List<TelemetryReadingDto> Readings { get; set; } = [];

    public List<DashboardAlertDto> Alerts { get; set; } = [];

    public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
}