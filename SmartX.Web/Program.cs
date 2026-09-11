using SmartX.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// SMART-X WEB APPLICATION
// ---------------------------------------------------------

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// ---------------------------------------------------------
// SMART-X API CLIENT
// ---------------------------------------------------------

var apiBaseUrl =
    builder.Configuration["SmartXApi:BaseUrl"]
    ?? throw new InvalidOperationException(
        "SmartXApi:BaseUrl is not configured.");

builder.Services.AddHttpClient("SmartXApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
});

// ---------------------------------------------------------
// BUILD APPLICATION
// ---------------------------------------------------------

var app = builder.Build();

// ---------------------------------------------------------
// ERROR HANDLING
// ---------------------------------------------------------

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

// ---------------------------------------------------------
// HTTPS & SECURITY
// ---------------------------------------------------------

app.UseHttpsRedirection();
app.UseAntiforgery();

// ---------------------------------------------------------
// STATIC ASSETS & RAZOR COMPONENTS
// ---------------------------------------------------------

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();