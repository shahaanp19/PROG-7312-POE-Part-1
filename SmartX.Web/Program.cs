using SmartX.Web.Components;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// The API base URL is configuration-driven so that the frontend can communicate with different gateway instances without requiring source-code changes.

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


var app = builder.Build();


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


app.UseHttpsRedirection();
app.UseAntiforgery();


app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();

//References
//guardrex (2026). ASP.NET Core Blazor project structure. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/aspnet/core/blazor/project-structure?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//danroth27 (2023). Project structure for Blazor apps - .NET. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/architecture/blazor-for-web-forms-developers/project-structure [Accessed 12 Sept. 2026].
//karelz (2026). HttpClient.GetAsync Method (System.Net.Http). [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.getasync?view=net-10.0 [Accessed 12 Sept. 2026].
//IEvangelist (2025). Use the IHttpClientFactory - .NET. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory [Accessed 12 Sept. 2026].