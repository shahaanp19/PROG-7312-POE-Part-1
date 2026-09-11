using System.ComponentModel.DataAnnotations;
using SmartX.Shared.Enums;

namespace SmartX.Shared.Models;

public class Attachment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SensorDeviceId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string StoredFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public AttachmentType Type { get; set; }

    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;

    public string FilePath { get; set; } = string.Empty;
}