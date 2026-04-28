using ApplicationService.Application.DTOs;
using ApplicationService.Application.Services;
using BuildingBlocks.Security.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationService.API.Controllers;

[ApiController]
[Route("api/v1/applications")]
[Produces("application/json")]
public class ApplicationsController(ApplicationApplicationService applicationService, ICurrentUserAccessor currentUser) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Candidate")]
    [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> SubmitApplication([FromBody] SubmitApplicationRequest request, CancellationToken ct)
    {
        var result = await applicationService.SubmitApplicationAsync(currentUser.UserId, request, ct);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.Error == BuildingBlocks.Common.Results.Error.Conflict ? Conflict(new { result.Error.Code, result.Error.Message })
            : result.Error == BuildingBlocks.Common.Results.Error.NotFound ? NotFound(new { result.Error.Code, result.Error.Message })
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }

    [HttpGet("my")]
    [Authorize(Roles = "Candidate")]
    [ProducesResponseType(typeof(IReadOnlyList<ApplicationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyApplications(CancellationToken ct)
    {
        var result = await applicationService.GetMyApplicationsAsync(currentUser.UserId, ct);
        return Ok(result.Value);
    }

    [HttpGet("job/{jobId:guid}")]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(IReadOnlyList<ApplicationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplicationsForJob(Guid jobId, CancellationToken ct)
    {
        var result = await applicationService.GetApplicationsForJobAsync(jobId, currentUser.UserId, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error == BuildingBlocks.Common.Results.Error.Forbidden ? Forbid()
            : NotFound(new { result.Error.Code, result.Error.Message });
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplicationById(Guid id, CancellationToken ct)
    {
        var result = await applicationService.GetApplicationByIdAsync(id, currentUser.UserId, currentUser.Role, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error == BuildingBlocks.Common.Results.Error.Forbidden ? Forbid()
            : NotFound();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateApplicationStatusRequest request, CancellationToken ct)
    {
        var result = await applicationService.UpdateStatusAsync(id, currentUser.UserId, request, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error == BuildingBlocks.Common.Results.Error.Forbidden ? Forbid()
            : result.Error == BuildingBlocks.Common.Results.Error.NotFound ? NotFound()
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }

    [HttpPatch("{id:guid}/withdraw")]
    [Authorize(Roles = "Candidate")]
    [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Withdraw(Guid id, CancellationToken ct)
    {
        var result = await applicationService.WithdrawAsync(id, currentUser.UserId, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error == BuildingBlocks.Common.Results.Error.Forbidden ? Forbid()
            : result.Error == BuildingBlocks.Common.Results.Error.NotFound ? NotFound()
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }
}
