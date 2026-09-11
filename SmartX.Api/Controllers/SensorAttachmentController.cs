using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;
using SmartX.Shared.DTOs;
using System.Security.Cryptography;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/sensor-attachments")]
public sealed class SensorAttachmentController : ControllerBase
{
    private readonly SensorAttachmentService _attachmentService;
    private readonly SensorRegistryService _sensorRegistry;
    private readonly ILogger<SensorAttachmentController> _logger;

    public SensorAttachmentController(
        SensorAttachmentService attachmentService,
        SensorRegistryService sensorRegistry,
        ILogger<SensorAttachmentController> logger)
    {
        _attachmentService = attachmentService;
        _sensorRegistry = sensorRegistry;
        _logger = logger;
    }

    [HttpPost("{sensorDeviceId:guid}")]
    [RequestSizeLimit(11 * 1024 * 1024)]
    [RequestFormLimits(
        MultipartBodyLengthLimit = 11 * 1024 * 1024)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<SensorAttachmentResponse>> Upload(
        Guid sensorDeviceId,
        [FromForm] IFormFile file,
        [FromForm] string attachmentType = "Other",
        CancellationToken cancellationToken = default)
    {
        if (sensorDeviceId == Guid.Empty)
        {
            return BadRequest(new
            {
                message = "A valid sensor device ID is required."
            });
        }

        if (!_sensorRegistry.Contains(sensorDeviceId))
        {
            return NotFound(new
            {
                message = "Sensor was not found.",
                sensorDeviceId
            });
        }

        if (file is null)
        {
            return BadRequest(new
            {
                message = "A file is required."
            });
        }

        try
        {
            var attachment =
                await _attachmentService.SaveAsync(
                    sensorDeviceId,
                    file,
                    attachmentType,
                    cancellationToken);

            _logger.LogInformation(
                "Attachment {AttachmentId} uploaded for sensor {SensorDeviceId}.",
                attachment.AttachmentId,
                sensorDeviceId);

            return Ok(attachment);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "Attachment upload cancelled for sensor {SensorDeviceId}.",
                sensorDeviceId);

            return BadRequest(new
            {
                message = "The upload was cancelled."
            });
        }
        catch (IOException ex)
        {
            _logger.LogError(
                ex,
                "Attachment storage failed for sensor {SensorDeviceId}.",
                sensorDeviceId);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "The file could not be stored."
                });
        }
    }

    [HttpGet("{sensorDeviceId:guid}/{storedFileName}")]
    public async Task<IActionResult> Download(
        Guid sensorDeviceId,
        string storedFileName,
        CancellationToken cancellationToken = default)
    {
        if (sensorDeviceId == Guid.Empty)
        {
            return BadRequest(new
            {
                message = "A valid sensor device ID is required."
            });
        }

        if (!_sensorRegistry.Contains(sensorDeviceId))
        {
            return NotFound(new
            {
                message = "Sensor was not found.",
                sensorDeviceId
            });
        }

        try
        {
            var decryptedFile =
                await _attachmentService.ReadAndDecryptAsync(
                    sensorDeviceId,
                    storedFileName,
                    cancellationToken);

            HttpContext.Response.RegisterForDispose(
                decryptedFile);

            return File(
                decryptedFile,
                "application/octet-stream",
                "decrypted-attachment");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (FileNotFoundException)
        {
            return NotFound(new
            {
                message = "The requested attachment was not found."
            });
        }
        catch (CryptographicException ex)
        {
            _logger.LogWarning(
                ex,
                "Attachment authentication failed for sensor {SensorDeviceId}.",
                sensorDeviceId);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "The attachment failed integrity verification."
                });
        }
        catch (OperationCanceledException)
        {
            return BadRequest(new
            {
                message = "The download was cancelled."
            });
        }
        catch (IOException ex)
        {
            _logger.LogError(
                ex,
                "Attachment retrieval failed for sensor {SensorDeviceId}.",
                sensorDeviceId);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "The attachment could not be retrieved."
                });
        }
    }
}