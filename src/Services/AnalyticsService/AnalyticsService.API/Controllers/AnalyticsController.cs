using BuildingBlocks.Security.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AnalyticsService.Application.DTOs;
using AnalyticsService.Application.Services;

namespace AnalyticsService.API.Controllers;

[ApiController]
[Route("api/v1/analytics")]
[Produces("application/json")]
public class AnalyticsController(AnalyticsAppService analyticsService, ICurrentUserAccessor currentUser) : ControllerBase
{
    [HttpGet("jobs/{jobId:guid}")]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(JobStatResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJobStats(Guid jobId, CancellationToken ct)
    {
        var result = await analyticsService.GetJobStatsAsync(jobId, currentUser.UserId, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error == BuildingBlocks.Common.Results.Error.Forbidden ? Forbid()
            : NotFound();
    }

    [HttpGet("recruiter/dashboard")]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(RecruiterDashboardResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecruiterDashboard(CancellationToken ct)
    {
        var result = await analyticsService.GetRecruiterDashboardAsync(currentUser.UserId, ct);
        return Ok(result.Value);
    }

    [HttpGet("platform")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IReadOnlyList<PlatformStatResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlatformStats([FromQuery] int days = 30, CancellationToken ct = default)
    {
        var result = await analyticsService.GetPlatformStatsAsync(days, ct);
        return Ok(result.Value);
    }
}
