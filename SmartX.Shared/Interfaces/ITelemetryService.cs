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


//References
//BillWagner (2023). Interfaces - define behavior for multiple types. [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces [Accessed 13 Sept. 2026].
//BillWagner (n.d.). Asynchronous programming in C#. [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/ [Accessed 13 Sept. 2026].
//BillWagner (2022). Cancellation in Managed Threads - .NET. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads [Accessed 13 Sept. 2026].