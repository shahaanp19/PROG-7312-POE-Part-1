using SmartX.Shared.Models;

namespace SmartX.Shared.Models;

public sealed class TelemetryBatch
{
    public Guid BatchId { get; init; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public List<TelemetryPacket<float>> Packets { get; init; } = new();

    public double TotalValue { get; private set; }

    public double AverageValue { get; private set; }

    public static TelemetryBatch FromPackets(
        List<TelemetryPacket<float>> packets)
    {
        ArgumentNullException.ThrowIfNull(packets);

        var batch = new TelemetryBatch
        {
            Packets = packets
        };

        if (packets.Count > 0)
        {
            batch.TotalValue =
                packets.Sum(packet => packet.Value);

            batch.AverageValue =
                packets.Average(packet => packet.Value);
        }

        return batch;
    }
}