using System.ComponentModel.DataAnnotations;
using SmartX.Shared.Enums;

namespace SmartX.Shared.DTOs;

public class AttachmentUploadRequest
{
    [Required]
    public Guid SensorDeviceId { get; set; }

    [Required]
    public AttachmentType Type { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }
}