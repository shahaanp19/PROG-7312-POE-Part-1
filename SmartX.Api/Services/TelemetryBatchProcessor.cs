using SmartX.Shared.Models;

namespace SmartX.Api.Services;

public sealed class TelemetryBatchProcessor
{
    public List<SmartX.Shared.Generics.TelemetryPacket<float>> ProcessHistoricalBatches(
        float[][] historicalBatches)
    {
        ArgumentNullException.ThrowIfNull(historicalBatches);

        var packets =
            new List<SmartX.Shared.Generics.TelemetryPacket<float>>();

        for (var batchIndex = 0;
             batchIndex < historicalBatches.Length;
             batchIndex++)
        {
            var batch = historicalBatches[batchIndex];

            if (batch is null || batch.Length == 0)
            {
                continue;
            }

            for (var readingIndex = 0;
                 readingIndex < batch.Length;
                 readingIndex++)
            {
                var packet =
                    new SmartX.Shared.Generics.TelemetryPacket<float>(
                        Guid.NewGuid(),
                        $"HistoricalTemperature_Batch{batchIndex + 1}_Reading{readingIndex + 1}",
                        batch[readingIndex],
                        "°C");

                packets.Add(packet);
            }
        }

        return packets;
    }

    public float[,] CreateTelemetryMatrix(
        float[][] historicalBatches)
    {
        ArgumentNullException.ThrowIfNull(historicalBatches);

        if (historicalBatches.Length == 0)
        {
            return new float[0, 0];
        }

        var rowCount = historicalBatches.Length;

        var columnCount = historicalBatches.Max(
            batch => batch?.Length ?? 0);

        var matrix = new float[rowCount, columnCount];

        for (var row = 0; row < rowCount; row++)
        {
            var batch = historicalBatches[row];

            if (batch is null)
            {
                continue;
            }

            for (var column = 0;
                 column < batch.Length;
                 column++)
            {
                matrix[row, column] = batch[column];
            }
        }

        return matrix;
    }
}