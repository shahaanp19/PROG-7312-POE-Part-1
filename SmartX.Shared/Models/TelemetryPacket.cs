namespace SmartX.Shared.Models;

public sealed class TelemetryPacket<T>
{
    public Guid PacketId { get; init; } = Guid.NewGuid();

    public Guid SensorDeviceId { get; init; }

    public string MetricName { get; init; } = string.Empty;

    public T Value { get; init; } = default!;

    public string Unit { get; init; } = string.Empty;

    public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;

    public TelemetryPacket(
        Guid sensorDeviceId,
        string metricName,
        T value,
        string unit)
    {
        SensorDeviceId = sensorDeviceId;
        MetricName = metricName;
        Value = value;
        Unit = unit;
    }
}

//References
//BillWagner (n.d.). Properties - C# Programming Guide. [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties [Accessed 12 Sept. 2026].
//BillWagner (2024). The init keyword - init only properties - C# reference. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/init [Accessed 12 Sept. 2026].
//BillWagner (n.d.). Classes and objects - C# Fundamentals tutorial. [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/classes [Accessed 12 Sept. 2026].
//dotnet-bot (2026). Array.Empty Method (System). [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.array.empty?view=net-10.0 [Accessed 12 Sept. 2026].
//StephenWalther (n.d.). Understanding Models, Views, and Controllers (C#). [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/overview/understanding-models-views-and-controllers-cs [Accessed 12 Sept. 2026].