using BuildingBlocks.Security.Extensions;
using InterviewService.Application.DTOs;
using InterviewService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewService.API.Controllers;

[ApiController]
[Route("api/v1/interviews")]
[Produces("application/json")]
public class InterviewsController(InterviewApplicationService interviewService, ICurrentUserAccessor currentUser) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(InterviewResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Schedule([FromBody] ScheduleInterviewRequest request, CancellationToken ct)
    {
        var result = await interviewService.ScheduleInterviewAsync(currentUser.UserId, request, ct);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.Error == BuildingBlocks.Common.Results.Error.Forbidden ? Forbid()
            : result.Error == BuildingBlocks.Common.Results.Error.Conflict ? Conflict(new { result.Error.Code, result.Error.Message })
            : result.Error == BuildingBlocks.Common.Results.Error.NotFound ? NotFound(new { result.Error.Code, result.Error.Message })
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }

    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyList<InterviewResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyInterviews(CancellationToken ct)
    {
        var result = await interviewService.GetMyInterviewsAsync(currentUser.UserId, currentUser.Role, ct);
        return Ok(result.Value);
    }

    [HttpPatch("{id:guid}/confirm")]
    [Authorize(Roles = "Candidate")]
    [ProducesResponseType(typeof(InterviewResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken ct)
    {
        var result = await interviewService.ConfirmInterviewAsync(id, currentUser.UserId, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error == BuildingBlocks.Common.Results.Error.Forbidden ? Forbid()
            : NotFound();
    }

    [HttpPatch("{id:guid}/reschedule-request")]
    [Authorize(Roles = "Candidate")]
    [ProducesResponseType(typeof(InterviewResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RequestReschedule(Guid id, [FromBody] RequestRescheduleInterviewRequest request, CancellationToken ct)
    {
        var result = await interviewService.RequestRescheduleAsync(id, currentUser.UserId, request, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error == BuildingBlocks.Common.Results.Error.Forbidden ? Forbid()
            : NotFound();
    }

    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(InterviewResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var result = await interviewService.CancelInterviewAsync(id, currentUser.UserId, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error == BuildingBlocks.Common.Results.Error.Forbidden ? Forbid()
            : result.Error == BuildingBlocks.Common.Results.Error.NotFound ? NotFound()
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }
}
