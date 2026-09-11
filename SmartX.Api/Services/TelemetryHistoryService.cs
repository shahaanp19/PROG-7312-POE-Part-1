namespace SmartX.Api.Services;

public class TelemetryHistoryService
{
    private readonly Dictionary<Guid, List<double>> _history = new();

    public void AddReading(Guid sensorId, double value)
    {
        if (!_history.TryGetValue(sensorId, out var readings))
        {
            readings = new List<double>();
            _history[sensorId] = readings;
        }

        readings.Add(value);
    }

    public IReadOnlyList<double> GetReadings(Guid sensorId)
    {
        return _history.TryGetValue(sensorId, out var readings)
            ? readings.AsReadOnly()
            : Array.Empty<double>();
    }

    public double[,] GetMatrix(
        Guid sensorId,
        int rows,
        int columns)
    {
        if (rows <= 0 || columns <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rows),
                "Rows and columns must be greater than zero.");
        }

        var readings = GetReadings(sensorId);

        var matrix = new double[rows, columns];

        var index = 0;

        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                matrix[row, column] =
                    index < readings.Count
                        ? readings[index]
                        : 0;

                index++;
            }
        }

        return matrix;
    }
}