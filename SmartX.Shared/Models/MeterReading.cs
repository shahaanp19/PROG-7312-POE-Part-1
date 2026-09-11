namespace SmartX.Shared.Models;

public readonly record struct MeterReading(
    Guid SensorDeviceId,
    double Value,
    string Unit)
{
    public static MeterReading operator +(
        MeterReading left,
        MeterReading right)
    {
        ValidateCompatibleReadings(left, right);

        return new MeterReading(
            left.SensorDeviceId,
            left.Value + right.Value,
            left.Unit);
    }

    public static MeterReading operator -(
        MeterReading left,
        MeterReading right)
    {
        ValidateCompatibleReadings(left, right);

        return new MeterReading(
            left.SensorDeviceId,
            left.Value - right.Value,
            left.Unit);
    }

    private static void ValidateCompatibleReadings(
        MeterReading left,
        MeterReading right)
    {
        if (left.SensorDeviceId != right.SensorDeviceId)
        {
            throw new InvalidOperationException(
                "Meter readings must belong to the same sensor.");
        }

        if (!string.Equals(
                left.Unit,
                right.Unit,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Meter readings must use the same unit.");
        }
    }
}