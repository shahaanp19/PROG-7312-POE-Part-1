using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;
using SmartX.Shared.Models;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly DashboardEngagementService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        DashboardEngagementService dashboardService,
        ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

   

    [HttpGet("engagement")]
    public ActionResult<DashboardEngagementSnapshot>
        GetEngagementSnapshot()
    {
        var snapshot =
            _dashboardService.GetEngagementSnapshot();

        return Ok(snapshot);
    }


    [HttpGet("issues")]
    public ActionResult<IReadOnlyList<UserIssue>> GetIssues()
    {
        return Ok(
            _dashboardService.GetIssues());
    }


    [HttpGet("issues/count")]
    public IActionResult GetOpenIssueCount()
    {
        return Ok(new
        {
            openIssues =
                _dashboardService.GetOpenIssueCount()
        });
    }


    [HttpPost("issues")]
    public ActionResult<UserIssue> CreateIssue(
        [FromBody] UserIssue request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (request is null)
        {
            return BadRequest(new
            {
                message = "An issue payload is required."
            });
        }

        try
        {
            var issue =
                _dashboardService.AddIssue(
                    request.Title,
                    request.Description,
                    request.Severity);

            _logger.LogInformation(
                "Dashboard issue {IssueId} created with severity {Severity}.",
                issue.Id,
                issue.Severity);

            return Created(
                $"/api/dashboard/issues/{issue.Id}",
                issue);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

  
    [HttpPut("issues/{issueId:guid}/resolve")]
    public IActionResult ResolveIssue(
        Guid issueId)
    {
        if (issueId == Guid.Empty)
        {
            return BadRequest(new
            {
                message = "A valid issue ID is required."
            });
        }

        var resolved =
            _dashboardService.ResolveIssue(issueId);

        if (!resolved)
        {
            return NotFound(new
            {
                message = "Issue was not found.",
                issueId
            });
        }

        _logger.LogInformation(
            "Dashboard issue {IssueId} resolved.",
            issueId);

        return Ok(new
        {
            issueId,
            status = "Resolved",
            resolvedAtUtc = DateTime.UtcNow
        });
    }
}

//References
//tdykstra (2024). Create web APIs with ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//wadepickett (2025). Tutorial: Create a controller-based web API with ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0&tabs=visual-studio [Accessed 12 Sept. 2026].
//tdykstra (2025). Model validation in ASP.NET Core MVC. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//tdykstra (2024). Dependency injection in ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].