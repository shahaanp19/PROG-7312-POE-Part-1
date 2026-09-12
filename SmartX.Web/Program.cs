using SmartX.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// SMART-X WEB APPLICATION
// =========================================================

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// =========================================================
// SMART-X API CLIENT
// =========================================================
// The API base URL is configuration-driven so that the
// frontend can communicate with different gateway instances
// without requiring source-code changes.

var apiBaseUrl =
    builder.Configuration["SmartXApi:BaseUrl"]
    ?? throw new InvalidOperationException(
        "SmartXApi:BaseUrl is not configured.");

if (!Uri.TryCreate(
        apiBaseUrl,
        UriKind.Absolute,
        out var apiBaseUri))
{
    throw new InvalidOperationException(
        "SmartXApi:BaseUrl must be a valid absolute URI.");
}

builder.Services.AddHttpClient("SmartXApi", client =>
{
    client.BaseAddress = apiBaseUri;

    client.Timeout = TimeSpan.FromSeconds(10);

    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
            "application/json"));
});

// =========================================================
// BUILD APPLICATION
// =========================================================

var app = builder.Build();

// =========================================================
// ERROR HANDLING
// =========================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

// =========================================================
// HTTPS & SECURITY
// =========================================================

app.UseHttpsRedirection();
app.UseAntiforgery();

// =========================================================
// STATIC ASSETS & RAZOR COMPONENTS
// =========================================================

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// =========================================================
// APPLICATION STARTUP
// =========================================================

app.Run();