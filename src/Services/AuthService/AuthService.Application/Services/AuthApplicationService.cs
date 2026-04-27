using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using AuthService.Domain.Repositories;
using BuildingBlocks.Common.Results;
using BuildingBlocks.Security.Jwt;
using BuildingBlocks.Security.Models;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AuthService.Application.Services;

public class AuthApplicationService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordHasher passwordHasher,
    IGitHubOAuthService gitHubService,
    IEventPublisher eventPublisher,
    IOptions<JwtSettings> jwtOptions)
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email, ct))
            return Result.Failure<AuthResponse>(Error.Custom("Auth.EmailTaken", "Email is already registered."));

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            return Result.Failure<AuthResponse>(Error.Custom("Auth.InvalidRole", "Invalid role specified."));

        if (role == UserRole.Admin)
            return Result.Failure<AuthResponse>(Error.Forbidden);

        var passwordHash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, passwordHash, request.FirstName, request.LastName, role);

        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await eventPublisher.PublishAsync(new BuildingBlocks.Contracts.Events.UserRegisteredEvent
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString()
        }, ct);

        return Result.Success(await BuildAuthResponseAsync(user, ct));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, ct);
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result.Failure<AuthResponse>(Error.Custom("Auth.InvalidCredentials", "Invalid email or password."));

        if (!user.IsActive)
            return Result.Failure<AuthResponse>(Error.Custom("Auth.AccountDisabled", "Account is disabled."));

        user.RecordLogin();
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(await BuildAuthResponseAsync(user, ct));
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        var principal = jwtTokenGenerator.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal is null)
            return Result.Failure<AuthResponse>(Error.Custom("Auth.InvalidToken", "Invalid access token."));

        var userId = Guid.Parse(principal.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);
        var storedToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);

        if (storedToken is null || storedToken.UserId != userId || !storedToken.IsActive)
            return Result.Failure<AuthResponse>(Error.Custom("Auth.InvalidRefreshToken", "Invalid or expired refresh token."));

        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null) return Result.Failure<AuthResponse>(Error.NotFound);

        storedToken.Revoke("Rotated", null);
        await refreshTokenRepository.UpdateAsync(storedToken, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(await BuildAuthResponseAsync(user, ct));
    }

    public async Task<Result> LogoutAsync(Guid userId, string refreshToken, CancellationToken ct = default)
    {
        var token = await refreshTokenRepository.GetByTokenAsync(refreshToken, ct);
        if (token is null || token.UserId != userId)
            return Result.Failure(Error.Custom("Auth.InvalidRefreshToken", "Refresh token not found."));

        token.Revoke("User logout");
        await refreshTokenRepository.UpdateAsync(token, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result<AuthResponse>> GitHubLoginAsync(string code, CancellationToken ct = default)
    {
        var githubUser = await gitHubService.GetUserInfoAsync(code, ct);
        if (githubUser is null)
            return Result.Failure<AuthResponse>(Error.Custom("Auth.GitHubFailed", "GitHub authentication failed."));

        var user = await userRepository.GetByExternalIdAsync(githubUser.Id, ct)
                   ?? await userRepository.GetByEmailAsync(githubUser.Email, ct);

        if (user is null)
        {
            var nameParts = githubUser.Name.Split(' ', 2);
            user = User.CreateOAuth(
                githubUser.Email,
                nameParts[0],
                nameParts.Length > 1 ? nameParts[1] : string.Empty,
                UserRole.Candidate,
                AuthProvider.GitHub,
                githubUser.Id);

            await userRepository.AddAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        return Result.Success(await BuildAuthResponseAsync(user, ct));
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(User user, CancellationToken ct)
    {
        var claims = new List<System.Security.Claims.Claim>
        {
            new(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(System.Security.Claims.ClaimTypes.Email, user.Email),
            new(System.Security.Claims.ClaimTypes.GivenName, user.FirstName),
            new(System.Security.Claims.ClaimTypes.Surname, user.LastName),
            new(System.Security.Claims.ClaimTypes.Role, user.Role.ToString()),
            new("provider", user.Provider.ToString())
        };

        var accessToken = jwtTokenGenerator.GenerateAccessToken(claims);
        var refreshTokenValue = jwtTokenGenerator.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(refreshTokenValue, user.Id, _jwtSettings.RefreshTokenExpiryDays);

        await refreshTokenRepository.AddAsync(refreshToken, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new AuthResponse(
            accessToken,
            refreshTokenValue,
            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            new UserDto(user.Id, user.Email, user.FirstName, user.LastName, user.FullName, user.Role.ToString(), user.IsActive, user.LastLoginAt)
        );
    }
}
