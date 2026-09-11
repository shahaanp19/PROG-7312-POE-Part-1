using SmartX.Shared.Models;

namespace SmartX.Shared.Interfaces;

public interface ISensorDeviceRepository
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
        SensorDevice device,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        SensorDevice device,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        SensorDevice device,
        CancellationToken cancellationToken = default);
}