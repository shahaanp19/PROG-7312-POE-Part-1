using SmartX.Api.Middleware;
using SmartX.Api.Services;
using SmartX.Shared.Validators;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();



builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type =>
        type.FullName!.Replace("+", "."));
});


// The policy is intentionally named so that the trusted frontend integration point is explicit and maintainable.

builder.Services.AddCors(options =>
{
    options.AddPolicy("SmartXWebClient", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddScoped<TelemetryThresholdValidator>();
builder.Services.AddScoped<TelemetryValidator>();
builder.Services.AddScoped<TelemetryIngestionService>();


builder.Services.AddSingleton<SensorRegistryService>();
builder.Services.AddSingleton<TelemetryHistoryService>();
builder.Services.AddSingleton<SensorService>();
builder.Services.AddSingleton<SensorAttachmentService>();
builder.Services.AddSingleton<DashboardEngagementService>();


builder.Services.AddSingleton<TelemetryBatchProcessor>();
builder.Services.AddSingleton<RecursiveDeploymentValidator>();


var app = builder.Build();


// This middleware provides a centralised safety boundary for unexpected API failures. This is for global exception handling. 

app.UseMiddleware<GlobalExceptionMiddleware>();


//Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("SmartXWebClient");

app.MapControllers();


// Dedicated health endpoint allows the frontend and future monitoring infrastructure to distinguish gateway health checks from normal API routes.

app.MapGet("/health", () =>
    Results.Ok(new
    {
        application = "Smart-X Data Ingestion Gateway",
        status = "Online",
        timestampUtc = DateTime.UtcNow
    }))
    .WithName("GetGatewayHealth");


// Retained for simple browser/API verification.

app.MapGet("/", () =>
    Results.Ok(new
    {
        application = "Smart-X Data Ingestion Gateway",
        status = "Online",
        timestampUtc = DateTime.UtcNow
    }))
    .WithName("GetGatewayStatus");

app.Run();

//References
//jongalloway (n.d.). Create a web API with ASP.NET Core controllers - Training. [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/training/modules/build-web-api-aspnet-core/ [Accessed 12 Sept. 2026].
//tdykstra (2024). Dependency injection in ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//gewarren (2026). Service lifetimes (dependency injection) - .NET. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/service-lifetimes [Accessed 12 Sept. 2026].
//gewarren (2026). Dependency injection - .NET. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/overview [Accessed 12 Sept. 2026].
//RicoSuter (2023). ASP.NET Core web API documentation with Swagger / OpenAPI. [online] learn.microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-8.0 [Accessed 12 Sept. 2026].
//RicoSuter (2026). ASP.NET Core web API documentation with Swagger / OpenAPI. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-8.0 [Accessed 12 Sept. 2026].
//tdykstra (2026). Enable Cross-Origin Requests (CORS) in ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//tdykstra (2025). ASP.NET Core Middleware. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].