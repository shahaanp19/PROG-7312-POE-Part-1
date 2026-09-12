using System.Net;
using System.Text.Json;

namespace SmartX.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(context);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context)
    {
        context.Response.StatusCode =
            (int)HttpStatusCode.InternalServerError;

        context.Response.ContentType = "application/json";

        var response = new
        {
            statusCode = context.Response.StatusCode,
            message = "An unexpected server error occurred.",
            timestampUtc = DateTime.UtcNow
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}


//References
//tdykstra (2025). ASP.NET Core Middleware. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//tdykstra (2025). Handle errors in ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//tdykstra (2026). Logging in .NET and ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//karelz (2026). HttpStatusCode Enum (System.Net). [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode?view=net-10.0 [Accessed 12 Sept. 2026].
//dotnet-bot (2026). System.Text.Json Namespace. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.text.json?view=net-11.0-pp [Accessed 12 Sept. 2026].