using SmartX.Shared.Enums;

namespace SmartX.Shared.DTOs;

public class SensorDeviceDto
{
    public Guid Id { get; set; }

    public string MacAddress { get; set; } = string.Empty;

    public string DeviceName { get; set; } = string.Empty;

    public SensorCategory Category { get; set; }

    public Guid DeploymentLocationId { get; set; }

    public string LocationName { get; set; } = string.Empty;

    public string NodeId { get; set; } = string.Empty;

    public DeviceStatus Status { get; set; }

    public DateTime RegisteredAtUtc { get; set; }

    public DateTime? LastTelemetryAtUtc { get; set; }
}