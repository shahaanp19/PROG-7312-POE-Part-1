using SmartX.Shared.Models;

namespace SmartX.Shared.Interfaces;

public interface ISensorRepository
{
    Task<SensorDevice?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SensorDevice?> GetByMacAddressAsync(
        string macAddress,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SensorDevice>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<SensorDevice> AddAsync(
        SensorDevice sensor,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        SensorDevice sensor,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        SensorDevice sensor,
        CancellationToken cancellationToken = default);
}


//References
//BillWagner (2023). Interfaces - define behavior for multiple types. [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces [Accessed 13 Sept. 2026].
//BillWagner (n.d.). Asynchronous programming in C#. [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/ [Accessed 13 Sept. 2026].
//BillWagner (2022). Cancellation in Managed Threads - .NET. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads [Accessed 13 Sept. 2026].