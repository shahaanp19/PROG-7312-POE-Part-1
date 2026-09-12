using SmartX.Shared.Enums;

namespace SmartX.Shared.Models;

public class TelemetryFilter
{
    public Guid? SensorDeviceId { get; set; }

    public string? MetricName { get; set; }

    public TelemetryDataType? DataType { get; set; }

    public DateTime? FromUtc { get; set; }

    public DateTime? ToUtc { get; set; }

    public double? MinimumValue { get; set; }

    public double? MaximumValue { get; set; }

    public bool AnomaliesOnly { get; set; }

    public AlertSeverity? MinimumSeverity { get; set; }

    public int Limit { get; set; } = 100;
}

//References
//BillWagner (n.d.). Properties - C# Programming Guide. [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties [Accessed 12 Sept. 2026].
//BillWagner (2024). The init keyword - init only properties - C# reference. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/init [Accessed 12 Sept. 2026].
//BillWagner (n.d.). Classes and objects - C# Fundamentals tutorial. [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/classes [Accessed 12 Sept. 2026].
//dotnet-bot (2026). Array.Empty Method (System). [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.array.empty?view=net-10.0 [Accessed 12 Sept. 2026].
//StephenWalther (n.d.). Understanding Models, Views, and Controllers (C#). [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/overview/understanding-models-views-and-controllers-cs [Accessed 12 Sept. 2026].