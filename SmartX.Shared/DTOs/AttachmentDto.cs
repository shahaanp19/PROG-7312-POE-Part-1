using SmartX.Shared.Enums;

namespace SmartX.Shared.DTOs;

public class AttachmentDto
{
    public Guid Id { get; set; }

    public Guid SensorDeviceId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public AttachmentType Type { get; set; }

    public DateTime UploadedAtUtc { get; set; }

    public string DownloadUrl { get; set; } = string.Empty;
}