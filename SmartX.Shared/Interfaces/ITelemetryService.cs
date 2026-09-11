using SmartX.Shared.DTOs;
using SmartX.Shared.Generics;

namespace SmartX.Shared.Interfaces;

public interface ITelemetryService
{
    Task<TelemetryIngestionResult> IngestAsync<T>(
        TelemetryPacket<T> packet,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TelemetryPacket<T>>> GetLatestAsync<T>(
        Guid sensorDeviceId,
        int count = 100,
        CancellationToken cancellationToken = default);
}