using SmartX.Api.Middleware;
using SmartX.Api.Services;
using SmartX.Shared.Validators;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// SMART-X DATA INGESTION GATEWAY
// =========================================================

// =========================================================
// CONTROLLERS
// =========================================================

builder.Services.AddControllers();

// =========================================================
// API DOCUMENTATION
// =========================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type =>
        type.FullName!.Replace("+", "."));
});

// =========================================================
// CORS
// =========================================================
// The policy is intentionally named so that the trusted
// frontend integration point is explicit and maintainable.

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

// =========================================================
// TELEMETRY VALIDATION & INGESTION SERVICES
// =========================================================

builder.Services.AddScoped<TelemetryThresholdValidator>();
builder.Services.AddScoped<TelemetryValidator>();
builder.Services.AddScoped<TelemetryIngestionService>();

// =========================================================
// SMART-X SENSOR & TELEMETRY SERVICES
// =========================================================

builder.Services.AddSingleton<SensorRegistryService>();
builder.Services.AddSingleton<TelemetryHistoryService>();
builder.Services.AddSingleton<SensorService>();
builder.Services.AddSingleton<SensorAttachmentService>();
builder.Services.AddSingleton<DashboardEngagementService>();

// =========================================================
// ADVANCED DATA PROCESSING SERVICES
// =========================================================

builder.Services.AddSingleton<TelemetryBatchProcessor>();
builder.Services.AddSingleton<RecursiveDeploymentValidator>();

// =========================================================
// BUILD APPLICATION
// =========================================================

var app = builder.Build();

// =========================================================
// GLOBAL EXCEPTION HANDLING
// =========================================================
// This middleware provides a centralised safety boundary for
// unexpected API failures.

app.UseMiddleware<GlobalExceptionMiddleware>();

// =========================================================
// SWAGGER
// =========================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =========================================================
// HTTP PIPELINE
// =========================================================

app.UseHttpsRedirection();

app.UseCors("SmartXWebClient");

app.MapControllers();

// =========================================================
// SMART-X GATEWAY HEALTH ENDPOINT
// =========================================================
// Dedicated health endpoint allows the frontend and future
// monitoring infrastructure to distinguish gateway health
// checks from normal API routes.

app.MapGet("/health", () =>
    Results.Ok(new
    {
        application = "Smart-X Data Ingestion Gateway",
        status = "Online",
        timestampUtc = DateTime.UtcNow
    }))
    .WithName("GetGatewayHealth");

// =========================================================
// SMART-X GATEWAY ROOT ENDPOINT
// =========================================================
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