using BuildingBlocks.Common.Pagination;
using BuildingBlocks.Security.Extensions;
using JobService.Application.DTOs;
using JobService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobService.API.Controllers;

[ApiController]
[Route("api/v1/jobs")]
[Produces("application/json")]
public class JobsController(JobApplicationService jobService, ICurrentUserAccessor currentUser) : ControllerBase
{
    /// <summary>Create a new job posting (Recruiter only).</summary>
    [HttpPost]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(JobResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobRequest request, CancellationToken ct)
    {
        var result = await jobService.CreateJobAsync(currentUser.UserId, request, ct);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }

    /// <summary>Update an existing job (Recruiter, owner only).</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(JobResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateJob(Guid id, [FromBody] UpdateJobRequest request, CancellationToken ct)
    {
        var result = await jobService.UpdateJobAsync(id, currentUser.UserId, request, ct);
        return result.IsSuccess ? Ok(result.Value)
            : result.Error == BuildingBlocks.Common.Results.Error.NotFound ? NotFound()
            : result.Error == BuildingBlocks.Common.Results.Error.Forbidden ? Forbid()
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }

    /// <summary>Pause an active job (Recruiter, owner only).</summary>
    [HttpPatch("{id:guid}/pause")]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PauseJob(Guid id, CancellationToken ct)
    {
        var result = await jobService.PauseJobAsync(id, currentUser.UserId, ct);
        return result.IsSuccess ? NoContent()
            : result.Error == BuildingBlocks.Common.Results.Error.NotFound ? NotFound()
            : result.Error == BuildingBlocks.Common.Results.Error.Forbidden ? Forbid()
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }

    /// <summary>Delete a job (Recruiter, owner only).</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteJob(Guid id, CancellationToken ct)
    {
        var result = await jobService.DeleteJobAsync(id, currentUser.UserId, ct);
        return result.IsSuccess ? NoContent()
            : result.Error == BuildingBlocks.Common.Results.Error.NotFound ? NotFound()
            : result.Error == BuildingBlocks.Common.Results.Error.Forbidden ? Forbid()
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }

    /// <summary>Get all active jobs with pagination (public).</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponse<JobResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJobs(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var request = new JobSearchRequest(null, null, null, null, null, pageNumber, pageSize);
        var result = await jobService.SearchJobsAsync(request, ct);
        return Ok(result.Value);
    }

    /// <summary>Get a single job by ID (public).</summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(JobResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobById(Guid id, CancellationToken ct)
    {
        var result = await jobService.GetJobByIdAsync(id, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    /// <summary>Search and filter jobs (public).</summary>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponse<JobResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchJobs([FromQuery] JobSearchRequest request, CancellationToken ct)
    {
        var result = await jobService.SearchJobsAsync(request, ct);
        return Ok(result.Value);
    }
}
