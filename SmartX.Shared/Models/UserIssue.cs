namespace SmartX.Shared.Models;

public sealed class UserIssue
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Severity { get; set; } = "Info";

    public string Status { get; set; } = "Open";

    public DateTime ReportedAtUtc { get; set; } =
        DateTime.UtcNow;
}

//References
//BillWagner (n.d.). Properties - C# Programming Guide. [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties [Accessed 12 Sept. 2026].
//BillWagner (2024). The init keyword - init only properties - C# reference. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/init [Accessed 12 Sept. 2026].
//BillWagner (n.d.). Classes and objects - C# Fundamentals tutorial. [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/classes [Accessed 12 Sept. 2026].
//dotnet-bot (2026). Array.Empty Method (System). [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.array.empty?view=net-10.0 [Accessed 12 Sept. 2026].
//StephenWalther (n.d.). Understanding Models, Views, and Controllers (C#). [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/overview/understanding-models-views-and-controllers-cs [Accessed 12 Sept. 2026].