using System.Buffers.Binary;
using System.Security.Cryptography;
using SmartX.Shared.DTOs;

namespace SmartX.Api.Services;

public sealed class SensorAttachmentService
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".txt",
            ".log",
            ".json",
            ".xml",
            ".csv",
            ".pdf"
        };

    private const long MaximumFileSizeBytes = 10 * 1024 * 1024;

    private const int EncryptionChunkSize = 1024 * 1024;
    private const int NonceSize = 12;
    private const int TagSize = 16;

    private static readonly byte[] FileMagic =
    {
        (byte)'S',
        (byte)'X',
        (byte)'A',
        (byte)'E'
    };

    private const byte FileFormatVersion = 1;

    private readonly IWebHostEnvironment _environment;
    private readonly byte[] _encryptionKey;

    public SensorAttachmentService(
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        _environment = environment;

        var encodedKey =
            configuration["AttachmentEncryption:Key"];

        if (string.IsNullOrWhiteSpace(encodedKey))
        {
            throw new InvalidOperationException(
                "Attachment encryption key is not configured. " +
                "Configure AttachmentEncryption:Key using User Secrets.");
        }

        try
        {
            _encryptionKey =
                Convert.FromBase64String(encodedKey);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException(
                "Attachment encryption key must be valid Base64.",
                ex);
        }

        if (_encryptionKey.Length != 32)
        {
            throw new InvalidOperationException(
                "Attachment encryption key must decode to exactly 32 bytes.");
        }
    }

    public async Task<SensorAttachmentResponse> SaveAsync(
        Guid sensorDeviceId,
        IFormFile file,
        string attachmentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        ValidateSensorId(sensorDeviceId);
        ValidateUploadedFile(file);

        var extension =
            Path.GetExtension(file.FileName);

        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException(
                $"File type '{extension}' is not supported.");
        }

        var attachmentId = Guid.NewGuid();

        var uploadDirectory = Path.Combine(
            _environment.ContentRootPath,
            "App_Data",
            "SensorAttachments",
            sensorDeviceId.ToString("N"));

        Directory.CreateDirectory(uploadDirectory);

        var storedFileName =
            $"{attachmentId:N}.sxae";

        var storedFilePath =
            Path.Combine(
                uploadDirectory,
                storedFileName);

        try
        {
            await EncryptFileAsync(
                file,
                storedFilePath,
                cancellationToken);
        }
        catch
        {
            TryDeleteFile(storedFilePath);
            throw;
        }

        return new SensorAttachmentResponse
        {
            AttachmentId = attachmentId,
            SensorDeviceId = sensorDeviceId,

            OriginalFileName =
                Path.GetFileName(file.FileName),

            StoredFileName =
                storedFileName,

            ContentType =
                "application/octet-stream",

            FileSizeBytes =
                file.Length,

            AttachmentType =
                string.IsNullOrWhiteSpace(attachmentType)
                    ? "Other"
                    : attachmentType.Trim(),

            UploadedAtUtc =
                DateTime.UtcNow
        };
    }

    public async Task<Stream> ReadAndDecryptAsync(
        Guid sensorDeviceId,
        string storedFileName,
        CancellationToken cancellationToken = default)
    {
        ValidateSensorId(sensorDeviceId);

        if (string.IsNullOrWhiteSpace(storedFileName))
        {
            throw new ArgumentException(
                "A stored file name is required.",
                nameof(storedFileName));
        }

        var safeFileName =
            Path.GetFileName(storedFileName);

        if (!string.Equals(
                safeFileName,
                storedFileName,
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "Invalid stored file name.",
                nameof(storedFileName));
        }

        if (!safeFileName.EndsWith(
                ".sxae",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "The requested attachment is not a valid encrypted file.",
                nameof(storedFileName));
        }

        var filePath = Path.Combine(
            _environment.ContentRootPath,
            "App_Data",
            "SensorAttachments",
            sensorDeviceId.ToString("N"),
            safeFileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "The requested attachment was not found.",
                filePath);
        }

        var decryptedStream =
            new MemoryStream();

        try
        {
            await using var sourceStream =
                new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 64 * 1024,
                    options:
                        FileOptions.Asynchronous |
                        FileOptions.SequentialScan);

            await DecryptFileAsync(
                sourceStream,
                decryptedStream,
                cancellationToken);

            decryptedStream.Position = 0;

            return decryptedStream;
        }
        catch
        {
            await decryptedStream.DisposeAsync();
            throw;
        }
    }

    private async Task EncryptFileAsync(
        IFormFile file,
        string destinationPath,
        CancellationToken cancellationToken)
    {
        await using var sourceStream =
            file.OpenReadStream();

        await using var destinationStream =
            new FileStream(
                destinationPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 64 * 1024,
                options:
                    FileOptions.Asynchronous |
                    FileOptions.SequentialScan);

        await destinationStream.WriteAsync(
            FileMagic,
            cancellationToken);

        await destinationStream.WriteAsync(
            new[] { FileFormatVersion },
            cancellationToken);

        var plaintextBuffer =
            new byte[EncryptionChunkSize];

        var ciphertextBuffer =
            new byte[EncryptionChunkSize];

        var nonce =
            new byte[NonceSize];

        var tag =
            new byte[TagSize];

        using var aesGcm =
            new AesGcm(
                _encryptionKey,
                TagSize);

        while (true)
        {
            var bytesRead = 0;

            while (bytesRead < EncryptionChunkSize)
            {
                var read =
                    await sourceStream.ReadAsync(
                        plaintextBuffer.AsMemory(
                            bytesRead,
                            EncryptionChunkSize - bytesRead),
                        cancellationToken);

                if (read == 0)
                {
                    break;
                }

                bytesRead += read;
            }

            if (bytesRead == 0)
            {
                break;
            }

            RandomNumberGenerator.Fill(nonce);

            aesGcm.Encrypt(
                nonce,
                plaintextBuffer.AsSpan(
                    0,
                    bytesRead),
                ciphertextBuffer.AsSpan(
                    0,
                    bytesRead),
                tag);

            var lengthBytes =
                new byte[sizeof(int)];

            BinaryPrimitives.WriteInt32LittleEndian(
                lengthBytes,
                bytesRead);

            await destinationStream.WriteAsync(
                lengthBytes,
                cancellationToken);

            await destinationStream.WriteAsync(
                nonce,
                cancellationToken);

            await destinationStream.WriteAsync(
                ciphertextBuffer.AsMemory(
                    0,
                    bytesRead),
                cancellationToken);

            await destinationStream.WriteAsync(
                tag,
                cancellationToken);

            if (bytesRead < EncryptionChunkSize)
            {
                break;
            }
        }

        await destinationStream.FlushAsync(
            cancellationToken);
    }

    private async Task DecryptFileAsync(
        Stream sourceStream,
        Stream destinationStream,
        CancellationToken cancellationToken)
    {
        var magic =
            new byte[FileMagic.Length];

        await ReadExactlyAsync(
            sourceStream,
            magic,
            cancellationToken);

        if (!CryptographicOperations.FixedTimeEquals(
                magic,
                FileMagic))
        {
            throw new CryptographicException(
                "Invalid encrypted attachment format.");
        }

        var versionBuffer =
            new byte[1];

        await ReadExactlyAsync(
            sourceStream,
            versionBuffer,
            cancellationToken);

        if (versionBuffer[0] != FileFormatVersion)
        {
            throw new CryptographicException(
                "Unsupported encrypted attachment version.");
        }

        var lengthBuffer =
            new byte[sizeof(int)];

        var nonce =
            new byte[NonceSize];

        var tag =
            new byte[TagSize];

        var ciphertextBuffer =
            new byte[EncryptionChunkSize];

        var plaintextBuffer =
            new byte[EncryptionChunkSize];

        using var aesGcm =
            new AesGcm(
                _encryptionKey,
                TagSize);

        while (true)
        {
            var firstByte =
                await sourceStream.ReadAsync(
                    lengthBuffer.AsMemory(0, 1),
                    cancellationToken);

            if (firstByte == 0)
            {
                break;
            }

            await ReadExactlyAsync(
                sourceStream,
                lengthBuffer.AsMemory(1, 3),
                cancellationToken);

            var plaintextLength =
                BinaryPrimitives.ReadInt32LittleEndian(
                    lengthBuffer);

            if (plaintextLength <= 0 ||
                plaintextLength > EncryptionChunkSize)
            {
                throw new CryptographicException(
                    "Invalid encrypted attachment chunk length.");
            }

            await ReadExactlyAsync(
                sourceStream,
                nonce,
                cancellationToken);

            await ReadExactlyAsync(
                sourceStream,
                ciphertextBuffer.AsMemory(
                    0,
                    plaintextLength),
                cancellationToken);

            await ReadExactlyAsync(
                sourceStream,
                tag,
                cancellationToken);

            aesGcm.Decrypt(
                nonce,
                ciphertextBuffer.AsSpan(
                    0,
                    plaintextLength),
                tag,
                plaintextBuffer.AsSpan(
                    0,
                    plaintextLength));

            await destinationStream.WriteAsync(
                plaintextBuffer.AsMemory(
                    0,
                    plaintextLength),
                cancellationToken);
        }

        await destinationStream.FlushAsync(
            cancellationToken);
    }

    private static async Task ReadExactlyAsync(
        Stream stream,
        Memory<byte> buffer,
        CancellationToken cancellationToken)
    {
        var totalRead = 0;

        while (totalRead < buffer.Length)
        {
            var read =
                await stream.ReadAsync(
                    buffer[totalRead..],
                    cancellationToken);

            if (read == 0)
            {
                throw new CryptographicException(
                    "Encrypted attachment is incomplete.");
            }

            totalRead += read;
        }
    }

    private static void ValidateSensorId(
        Guid sensorDeviceId)
    {
        if (sensorDeviceId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid sensor device ID is required.",
                nameof(sensorDeviceId));
        }
    }

    private static void ValidateUploadedFile(
        IFormFile file)
    {
        if (file.Length <= 0)
        {
            throw new InvalidOperationException(
                "The uploaded file is empty.");
        }

        if (file.Length > MaximumFileSizeBytes)
        {
            throw new InvalidOperationException(
                "The uploaded file exceeds the 10 MB limit.");
        }
    }

    private static void TryDeleteFile(
        string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch
        {
            // Preserve the original upload exception.
        }
    }
}