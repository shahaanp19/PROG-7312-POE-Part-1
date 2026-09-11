using System.Globalization;

namespace SmartX.Shared.DataStructures;

public readonly struct TelemetryValue<T>
{
    public T Value { get; }

    public TelemetryValue(T value)
    {
        Value = value;
    }

    public static TelemetryValue<T> operator +(
        TelemetryValue<T> left,
        TelemetryValue<T> right)
    {
        if (typeof(T) == typeof(int))
        {
            int result =
                Convert.ToInt32(left.Value, CultureInfo.InvariantCulture) +
                Convert.ToInt32(right.Value, CultureInfo.InvariantCulture);

            return new TelemetryValue<T>((T)(object)result);
        }

        if (typeof(T) == typeof(float))
        {
            float result =
                Convert.ToSingle(left.Value, CultureInfo.InvariantCulture) +
                Convert.ToSingle(right.Value, CultureInfo.InvariantCulture);

            return new TelemetryValue<T>((T)(object)result);
        }

        if (typeof(T) == typeof(double))
        {
            double result =
                Convert.ToDouble(left.Value, CultureInfo.InvariantCulture) +
                Convert.ToDouble(right.Value, CultureInfo.InvariantCulture);

            return new TelemetryValue<T>((T)(object)result);
        }

        throw new InvalidOperationException(
            $"The + operator is not supported for telemetry type {typeof(T).Name}.");
    }

    public static TelemetryValue<T> operator -(
        TelemetryValue<T> left,
        TelemetryValue<T> right)
    {
        if (typeof(T) == typeof(int))
        {
            int result =
                Convert.ToInt32(left.Value, CultureInfo.InvariantCulture) -
                Convert.ToInt32(right.Value, CultureInfo.InvariantCulture);

            return new TelemetryValue<T>((T)(object)result);
        }

        if (typeof(T) == typeof(float))
        {
            float result =
                Convert.ToSingle(left.Value, CultureInfo.InvariantCulture) -
                Convert.ToSingle(right.Value, CultureInfo.InvariantCulture);

            return new TelemetryValue<T>((T)(object)result);
        }

        if (typeof(T) == typeof(double))
        {
            double result =
                Convert.ToDouble(left.Value, CultureInfo.InvariantCulture) -
                Convert.ToDouble(right.Value, CultureInfo.InvariantCulture);

            return new TelemetryValue<T>((T)(object)result);
        }

        throw new InvalidOperationException(
            $"The - operator is not supported for telemetry type {typeof(T).Name}.");
    }

    public static bool operator >(
        TelemetryValue<T> left,
        TelemetryValue<T> right)
    {
        return Comparer<T>.Default.Compare(left.Value, right.Value) > 0;
    }

    public static bool operator <(
        TelemetryValue<T> left,
        TelemetryValue<T> right)
    {
        return Comparer<T>.Default.Compare(left.Value, right.Value) < 0;
    }

    public static bool operator >=(
        TelemetryValue<T> left,
        TelemetryValue<T> right)
    {
        return Comparer<T>.Default.Compare(left.Value, right.Value) >= 0;
    }

    public static bool operator <=(
        TelemetryValue<T> left,
        TelemetryValue<T> right)
    {
        return Comparer<T>.Default.Compare(left.Value, right.Value) <= 0;
    }

    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }
}