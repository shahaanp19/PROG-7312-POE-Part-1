using SmartX.Api.Middleware;
using SmartX.Api.Services;
using SmartX.Shared.Validators;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// CONTROLLERS
// =========================================================

builder.Services.AddControllers();

// =========================================================
// SWAGGER
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
// APPLICATION SERVICES
// =========================================================

builder.Services.AddScoped<TelemetryThresholdValidator>();
builder.Services.AddScoped<TelemetryValidator>();
builder.Services.AddScoped<TelemetryIngestionService>();

builder.Services.AddSingleton<SensorRegistryService>();
builder.Services.AddSingleton<TelemetryHistoryService>();
builder.Services.AddSingleton<SensorService>();
builder.Services.AddSingleton<SensorAttachmentService>();
builder.Services.AddSingleton<DashboardEngagementService>();

// =========================================================
// SECTION 4 SERVICES
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
// GATEWAY HEALTH / STATUS
// =========================================================

app.MapGet("/", () =>
    Results.Ok(new
    {
        application = "Smart-X Data Ingestion Gateway",
        status = "Online",
        timestampUtc = DateTime.UtcNow
    }));

app.Run();