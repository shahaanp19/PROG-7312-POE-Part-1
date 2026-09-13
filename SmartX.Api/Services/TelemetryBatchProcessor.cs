using SmartX.Shared.Generics;

namespace SmartX.Api.Services;

public sealed class TelemetryBatchProcessor
{
    /// <summary>
    /// Processes historical telemetry stored as a jagged array.
    /// The jagged structure preserves variable batch sizes without
    /// allocating unused values for shorter batches.
    /// </summary>
    public List<TelemetryPacket<float>> ProcessHistoricalBatches(
        float[][] historicalBatches)
    {
        ArgumentNullException.ThrowIfNull(historicalBatches);

        var totalReadings = 0;

        for (var batchIndex = 0;
             batchIndex < historicalBatches.Length;
             batchIndex++)
        {
            var batch = historicalBatches[batchIndex];

            if (batch is not null)
            {
                totalReadings += batch.Length;
            }
        }

        var packets = new List<TelemetryPacket<float>>(totalReadings);

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
                packets.Add(
                    new TelemetryPacket<float>(
                        Guid.NewGuid(),
                        $"HistoricalTemperature_Batch{batchIndex + 1}_Reading{readingIndex + 1}",
                        batch[readingIndex],
                        "°C"));
            }
        }

        return packets;
    }

    /// <summary>
    /// Converts variable-length telemetry batches into a rectangular
    /// multidimensional array for matrix-oriented processing.
    ///
    /// The matrix uses row-major access:
    /// row = telemetry batch
    /// column = reading within the batch
    ///
    /// Unused cells are left at the default float value of zero.
    /// </summary>
    public float[,] CreateTelemetryMatrix(
        float[][] historicalBatches)
    {
        ArgumentNullException.ThrowIfNull(historicalBatches);

        if (historicalBatches.Length == 0)
        {
            return new float[0, 0];
        }

        var rowCount = historicalBatches.Length;
        var columnCount = 0;

        for (var row = 0; row < rowCount; row++)
        {
            var batch = historicalBatches[row];

            if (batch is not null &&
                batch.Length > columnCount)
            {
                columnCount = batch.Length;
            }
        }

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

    /// <summary>
    /// Flattens variable-length historical telemetry into one contiguous
    /// one-dimensional buffer.
    ///
    /// This provides a compact representation for sequential processing,
    /// bulk validation and memory-efficient traversal.
    /// </summary>
    public float[] FlattenTelemetryBatches(
        float[][] historicalBatches)
    {
        ArgumentNullException.ThrowIfNull(historicalBatches);

        var totalReadings = 0;

        for (var batchIndex = 0;
             batchIndex < historicalBatches.Length;
             batchIndex++)
        {
            var batch = historicalBatches[batchIndex];

            if (batch is not null)
            {
                totalReadings += batch.Length;
            }
        }

        if (totalReadings == 0)
        {
            return Array.Empty<float>();
        }

        var flattened = new float[totalReadings];
        var destinationIndex = 0;

        for (var batchIndex = 0;
             batchIndex < historicalBatches.Length;
             batchIndex++)
        {
            var batch = historicalBatches[batchIndex];

            if (batch is null || batch.Length == 0)
            {
                continue;
            }

            Array.Copy(
                batch,
                0,
                flattened,
                destinationIndex,
                batch.Length);

            destinationIndex += batch.Length;
        }

        return flattened;
    }
}