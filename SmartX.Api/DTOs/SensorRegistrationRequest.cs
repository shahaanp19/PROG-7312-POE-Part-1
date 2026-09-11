using System.ComponentModel.DataAnnotations;

namespace SmartX.Shared.DTOs;

public class SensorRegistrationRequest
{
    [Required(ErrorMessage = "Sensor name is required.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "Sensor name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Device MAC address / unique identifier is required.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "Device identifier must be between 2 and 100 characters.")]
    public string DeviceIdentifier { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sensor category is required.")]
    [RegularExpression(
        "^(Environmental|Power Consumption|Actuator)$",
        ErrorMessage = "Please select a valid sensor category.")]
    public string SensorType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Deployment location is required.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "Deployment location must be between 2 and 100 characters.")]
    public string Location { get; set; } = string.Empty;

    [StringLength(
        500,
        ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}