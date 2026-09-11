namespace SmartX.Shared.DTOs;

public sealed class SensorAttachmentResponse
{
    public Guid AttachmentId { get; set; }

    public Guid SensorDeviceId { get; set; }

    public string OriginalFileName { get; set; } = string.Empty;

    public string StoredFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public string AttachmentType { get; set; } = string.Empty;

    public DateTime UploadedAtUtc { get; set; }

    public bool IsEncrypted { get; set; }

    public string EncryptionAlgorithm { get; set; } = string.Empty;
}