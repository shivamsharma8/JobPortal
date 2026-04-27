using AuthService.Application.DTOs;
using AuthService.Application.Services;
using BuildingBlocks.Security.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthController(AuthApplicationService authService, ICurrentUserAccessor currentUser) : ControllerBase
{
    /// <summary>Register a new Candidate or Recruiter account.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await authService.RegisterAsync(request, ct);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.Error.Code switch
            {
                "Auth.EmailTaken" => Conflict(new { result.Error.Code, result.Error.Message }),
                "Error.Forbidden" => Forbid(),
                _ => BadRequest(new { result.Error.Code, result.Error.Message })
            };
    }

    /// <summary>Login with email and password.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await authService.LoginAsync(request, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(new { result.Error.Code, result.Error.Message });
    }

    /// <summary>Refresh access token using a valid refresh token.</summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await authService.RefreshTokenAsync(request, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : Unauthorized(new { result.Error.Code, result.Error.Message });
    }

    /// <summary>Logout and revoke a refresh token.</summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
    {
        var result = await authService.LogoutAsync(currentUser.UserId, request.RefreshToken, ct);
        return result.IsSuccess ? NoContent() : BadRequest(new { result.Error.Code, result.Error.Message });
    }

    /// <summary>Get current authenticated user info.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        return Ok(new
        {
            UserId = currentUser.UserId,
            currentUser.Email,
            currentUser.Role,
            currentUser.IsAuthenticated
        });
    }

    /// <summary>GitHub OAuth2 callback endpoint.</summary>
    [HttpGet("github/callback")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GitHubCallback([FromQuery] string code, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(code)) return BadRequest(new { message = "Code is required." });
        var result = await authService.GitHubLoginAsync(code, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.Error.Code, result.Error.Message });
    }
}
