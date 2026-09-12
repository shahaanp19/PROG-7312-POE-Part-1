using System.ComponentModel.DataAnnotations;
using SmartX.Shared.Enums;

namespace SmartX.Shared.Models;

public class SensorDevice
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(17)]
    [RegularExpression(
        "^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$",
        ErrorMessage = "Enter a valid MAC address.")]
    public string MacAddress { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DeviceName { get; set; } = string.Empty;

    public SensorCategory Category { get; set; }

    public Guid DeploymentLocationId { get; set; }

    public DeploymentLocation? DeploymentLocation { get; set; }

    public DeviceStatus Status { get; set; } = DeviceStatus.Offline;

    public DateTime RegisteredAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? LastTelemetryAtUtc { get; set; }

    public List<Attachment> Attachments { get; set; } = [];

    public List<TelemetryReading> TelemetryReadings { get; set; } = [];
}

//References
//BillWagner (n.d.). Properties - C# Programming Guide. [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties [Accessed 12 Sept. 2026].
//BillWagner (2024). The init keyword - init only properties - C# reference. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/init [Accessed 12 Sept. 2026].
//BillWagner (n.d.). Classes and objects - C# Fundamentals tutorial. [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/classes [Accessed 12 Sept. 2026].
//dotnet-bot (2026). Array.Empty Method (System). [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.array.empty?view=net-10.0 [Accessed 12 Sept. 2026].
//StephenWalther (n.d.). Understanding Models, Views, and Controllers (C#). [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/overview/understanding-models-views-and-controllers-cs [Accessed 12 Sept. 2026].