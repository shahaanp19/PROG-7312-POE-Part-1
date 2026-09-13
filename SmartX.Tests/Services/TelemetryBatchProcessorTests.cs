using SmartX.Api.Services;

namespace SmartX.Tests.Services;

public class TelemetryBatchProcessorTests
{
    private readonly TelemetryBatchProcessor _processor = new();

    [Fact]
    public void ProcessHistoricalBatches_ShouldProcessAllTelemetryReadings()
    {
        // Arrange
        float[][] historicalBatches =
        {
            new[] { 20.5f, 21.0f },
            new[] { 22.5f },
            new[] { 23.0f, 24.5f, 25.0f }
        };

        // Act
        var result = _processor.ProcessHistoricalBatches(historicalBatches);

        // Assert
        Assert.Equal(6, result.Count);

        Assert.Equal(20.5f, result[0].Value);
        Assert.Equal(21.0f, result[1].Value);
        Assert.Equal(22.5f, result[2].Value);
        Assert.Equal(23.0f, result[3].Value);
        Assert.Equal(24.5f, result[4].Value);
        Assert.Equal(25.0f, result[5].Value);
    }

    [Fact]
    public void ProcessHistoricalBatches_ShouldPreserveBatchOrder()
    {
        // Arrange
        float[][] historicalBatches =
        {
            new[] { 10f, 20f },
            new[] { 30f, 40f }
        };

        // Act
        var result = _processor.ProcessHistoricalBatches(historicalBatches);

        // Assert
        Assert.Equal(10f, result[0].Value);
        Assert.Equal(20f, result[1].Value);
        Assert.Equal(30f, result[2].Value);
        Assert.Equal(40f, result[3].Value);
    }

    [Fact]
    public void ProcessHistoricalBatches_ShouldHandleEmptyBatches()
    {
        // Arrange
        float[][] historicalBatches =
        {
            Array.Empty<float>(),
            new[] { 15f },
            Array.Empty<float>()
        };

        // Act
        var result = _processor.ProcessHistoricalBatches(historicalBatches);

        // Assert
        var packet = Assert.Single(result);

        Assert.Equal(15f, packet.Value);
    }

    [Fact]
    public void ProcessHistoricalBatches_ShouldHandleNullInnerBatch()
    {
        // Arrange
        float[][] historicalBatches =
        {
            null!,
            new[] { 12f, 14f }
        };

        // Act
        var result = _processor.ProcessHistoricalBatches(historicalBatches);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(12f, result[0].Value);
        Assert.Equal(14f, result[1].Value);
    }

    [Fact]
    public void ProcessHistoricalBatches_ShouldThrowWhenInputIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _processor.ProcessHistoricalBatches(null!));
    }

    [Fact]
    public void CreateTelemetryMatrix_ShouldCreateCorrectDimensions()
    {
        // Arrange
        float[][] historicalBatches =
        {
            new[] { 1f, 2f, 3f },
            new[] { 4f, 5f },
            new[] { 6f, 7f, 8f }
        };

        // Act
        var result = _processor.CreateTelemetryMatrix(historicalBatches);

        // Assert
        Assert.Equal(3, result.GetLength(0));
        Assert.Equal(3, result.GetLength(1));
    }

    [Fact]
    public void CreateTelemetryMatrix_ShouldPreserveTelemetryValues()
    {
        // Arrange
        float[][] historicalBatches =
        {
            new[] { 10f, 20f },
            new[] { 30f, 40f, 50f }
        };

        // Act
        var result = _processor.CreateTelemetryMatrix(historicalBatches);

        // Assert
        Assert.Equal(10f, result[0, 0]);
        Assert.Equal(20f, result[0, 1]);

        Assert.Equal(30f, result[1, 0]);
        Assert.Equal(40f, result[1, 1]);
        Assert.Equal(50f, result[1, 2]);
    }

    [Fact]
    public void CreateTelemetryMatrix_ShouldLeaveUnusedCellsAsDefault()
    {
        // Arrange
        float[][] historicalBatches =
        {
            new[] { 1f },
            new[] { 2f, 3f, 4f }
        };

        // Act
        var result = _processor.CreateTelemetryMatrix(historicalBatches);

        // Assert
        Assert.Equal(0f, result[0, 1]);
        Assert.Equal(0f, result[0, 2]);
    }

    [Fact]
    public void CreateTelemetryMatrix_ShouldReturnEmptyMatrixForEmptyInput()
    {
        // Act
        var result =
            _processor.CreateTelemetryMatrix(Array.Empty<float[]>());

        // Assert
        Assert.Equal(0, result.GetLength(0));
        Assert.Equal(0, result.GetLength(1));
    }

    [Fact]
    public void CreateTelemetryMatrix_ShouldThrowWhenInputIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _processor.CreateTelemetryMatrix(null!));
    }

    [Fact]
    public void FlattenTelemetryBatches_ShouldCreateContiguousBuffer()
    {
        // Arrange
        float[][] historicalBatches =
        {
            new[] { 1f, 2f },
            new[] { 3f },
            new[] { 4f, 5f, 6f }
        };

        // Act
        var result =
            _processor.FlattenTelemetryBatches(historicalBatches);

        // Assert
        Assert.Equal(
            new[] { 1f, 2f, 3f, 4f, 5f, 6f },
            result);
    }

    [Fact]
    public void FlattenTelemetryBatches_ShouldPreserveReadingOrder()
    {
        // Arrange
        float[][] historicalBatches =
        {
            new[] { 10f, 20f },
            new[] { 30f, 40f },
            new[] { 50f }
        };

        // Act
        var result =
            _processor.FlattenTelemetryBatches(historicalBatches);

        // Assert
        Assert.Equal(10f, result[0]);
        Assert.Equal(20f, result[1]);
        Assert.Equal(30f, result[2]);
        Assert.Equal(40f, result[3]);
        Assert.Equal(50f, result[4]);
    }

    [Fact]
    public void FlattenTelemetryBatches_ShouldHandleEmptyBatches()
    {
        // Arrange
        float[][] historicalBatches =
        {
            Array.Empty<float>(),
            new[] { 5f },
            Array.Empty<float>()
        };

        // Act
        var result =
            _processor.FlattenTelemetryBatches(historicalBatches);

        // Assert
        Assert.Single(result);
        Assert.Equal(5f, result[0]);
    }

    [Fact]
    public void FlattenTelemetryBatches_ShouldReturnEmptyArrayForEmptyInput()
    {
        // Act
        var result =
            _processor.FlattenTelemetryBatches(Array.Empty<float[]>());

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FlattenTelemetryBatches_ShouldThrowWhenInputIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _processor.FlattenTelemetryBatches(null!));
    }
}