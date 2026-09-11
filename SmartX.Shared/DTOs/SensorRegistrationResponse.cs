namespace SmartX.Shared.DTOs;

public class SensorRegistrationResponse
{
    public Guid SensorId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string DeviceIdentifier { get; set; } = string.Empty;

    public string SensorType { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime RegisteredAtUtc { get; set; }
}