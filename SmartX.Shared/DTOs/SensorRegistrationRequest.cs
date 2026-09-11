using System.ComponentModel.DataAnnotations;

namespace SmartX.Shared.DTOs;

public class SensorRegistrationRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string DeviceIdentifier { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string SensorType { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}