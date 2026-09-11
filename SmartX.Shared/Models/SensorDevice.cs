using System.ComponentModel.DataAnnotations;
using SmartX.Shared.Enums;

namespace SmartX.Shared.Models;

public class SensorDevice
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(17)]
    [RegularExpression(
        "^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$",
        ErrorMessage = "Enter a valid MAC address.")]
    public string MacAddress { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DeviceName { get; set; } = string.Empty;

    public SensorCategory Category { get; set; }

    public Guid DeploymentLocationId { get; set; }

    public DeploymentLocation? DeploymentLocation { get; set; }

    public DeviceStatus Status { get; set; } = DeviceStatus.Offline;

    public DateTime RegisteredAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? LastTelemetryAtUtc { get; set; }

    public List<Attachment> Attachments { get; set; } = [];

    public List<TelemetryReading> TelemetryReadings { get; set; } = [];
}