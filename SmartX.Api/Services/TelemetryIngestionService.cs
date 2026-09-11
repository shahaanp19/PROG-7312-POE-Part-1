using System.Diagnostics;
using SmartX.Shared.DTOs;
using SmartX.Shared.Models;
using SmartX.Shared.Validators;

namespace SmartX.Api.Services;

public class TelemetryIngestionService
{
    private readonly TelemetryValidator _validator;

    public TelemetryIngestionService(TelemetryValidator validator)
    {
        _validator = validator;
    }

    public TelemetryIngestionResult Process(
        TelemetryMetric metric,
        double value)
    {
        ArgumentNullException.ThrowIfNull(metric);

        var stopwatch = Stopwatch.StartNew();

        var validationResult = _validator.Validate(metric, value);

        stopwatch.Stop();

        return new TelemetryIngestionResult
        {
            Success = validationResult.IsValid,
            PacketId = Guid.NewGuid(),
            SensorDeviceId = metric.SensorDeviceId,
            Message = validationResult.Message ?? string.Empty,
            IsAnomaly = validationResult.IsAnomaly,
            ProcessingTimeMilliseconds =
                stopwatch.Elapsed.TotalMilliseconds,
            ProcessedAtUtc = DateTime.UtcNow
        };
    }
}