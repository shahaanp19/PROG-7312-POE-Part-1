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