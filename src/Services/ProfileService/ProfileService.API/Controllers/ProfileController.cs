using BuildingBlocks.Security.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Application.DTOs;
using ProfileService.Application.Services;

namespace ProfileService.API.Controllers;

[ApiController]
[Route("api/v1/profile")]
[Authorize]
[Produces("application/json")]
public class ProfileController(ProfileApplicationService profileService, ICurrentUserAccessor currentUser) : ControllerBase
{
    /// <summary>Get current user's profile (Candidate or Recruiter).</summary>
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        if (currentUser.Role == "Candidate")
        {
            var result = await profileService.GetCandidateProfileAsync(currentUser.UserId, ct);
            return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Error.Code, result.Error.Message });
        }
        else
        {
            var result = await profileService.GetRecruiterProfileAsync(currentUser.UserId, ct);
            return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Error.Code, result.Error.Message });
        }
    }

    /// <summary>Update current user's profile.</summary>
    [HttpPut("me/candidate")]
    [Authorize(Roles = "Candidate")]
    [ProducesResponseType(typeof(CandidateProfileResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCandidateProfile([FromBody] UpdateCandidateProfileRequest request, CancellationToken ct)
    {
        var result = await profileService.UpdateCandidateProfileAsync(currentUser.UserId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.Error.Code, result.Error.Message });
    }

    /// <summary>Update recruiter company profile.</summary>
    [HttpPut("me/recruiter")]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(RecruiterProfileResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRecruiterProfile([FromBody] UpdateRecruiterProfileRequest request, CancellationToken ct)
    {
        var result = await profileService.UpdateRecruiterProfileAsync(currentUser.UserId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { result.Error.Code, result.Error.Message });
    }

    /// <summary>Upload resume document.</summary>
    [HttpPost("resume")]
    [Authorize(Roles = "Candidate")]
    [ProducesResponseType(typeof(ResumeDocumentDto), StatusCodes.Status201Created)]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> UploadResume(IFormFile file, [FromQuery] bool isPrimary = false, CancellationToken ct = default)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "File is required." });

        if (!new[] { "application/pdf", "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }.Contains(file.ContentType))
            return BadRequest(new { message = "Only PDF and Word documents are accepted." });

        var request = new UploadResumeRequest(file.FileName, file.ContentType, file.Length, isPrimary, file.OpenReadStream());
        var result = await profileService.UploadResumeAsync(currentUser.UserId, request, ct);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }

    /// <summary>Delete a resume document.</summary>
    [HttpDelete("resume/{resumeId:guid}")]
    [Authorize(Roles = "Candidate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteResume(Guid resumeId, CancellationToken ct)
    {
        var result = await profileService.DeleteResumeAsync(currentUser.UserId, resumeId, ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.Error.Code, result.Error.Message });
    }

    /// <summary>Add address to current user's profile.</summary>
    [HttpPost("address")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddAddress([FromBody] AddAddressRequest request, CancellationToken ct)
    {
        var result = await profileService.AddCandidateAddressAsync(currentUser.UserId, request, ct);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }

    /// <summary>Update an existing address.</summary>
    [HttpPut("address/{id:guid}")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateAddressRequest request, CancellationToken ct)
    {
        var result = await profileService.UpdateAddressAsync(id, request, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Error.Code, result.Error.Message });
    }
}
