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

    // =========================================================
    // GET ENGAGEMENT SNAPSHOT
    // =========================================================

    [HttpGet("engagement")]
    public ActionResult<DashboardEngagementSnapshot>
        GetEngagementSnapshot()
    {
        var snapshot =
            _dashboardService.GetEngagementSnapshot();

        return Ok(snapshot);
    }

    // =========================================================
    // GET USER-REPORTED ISSUES
    // =========================================================

    [HttpGet("issues")]
    public ActionResult<IReadOnlyList<UserIssue>> GetIssues()
    {
        return Ok(
            _dashboardService.GetIssues());
    }

    // =========================================================
    // GET OPEN ISSUE COUNT
    // =========================================================

    [HttpGet("issues/count")]
    public IActionResult GetOpenIssueCount()
    {
        return Ok(new
        {
            openIssues =
                _dashboardService.GetOpenIssueCount()
        });
    }

    // =========================================================
    // REPORT NEW ISSUE
    // =========================================================

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

    // =========================================================
    // RESOLVE ISSUE
    // =========================================================

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