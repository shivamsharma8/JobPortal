using BuildingBlocks.Common.Domain;
using AuthService.Domain.Enums;

namespace AuthService.Domain.Entities;

public sealed class User : BaseEntity
{
    private User() { }

    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public UserRole Role { get; private set; }
    public AuthProvider Provider { get; private set; } = AuthProvider.Local;
    public string? ExternalId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsEmailVerified { get; private set; } = false;
    public DateTime? LastLoginAt { get; private set; }

    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    private readonly List<RefreshToken> _refreshTokens = [];

    public static User Create(string email, string passwordHash, string firstName, string lastName, UserRole role)
    {
        var user = new User
        {
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            Provider = AuthProvider.Local
        };
        user.AddDomainEvent(new Events.UserCreatedDomainEvent(user.Id, user.Email, user.Role.ToString()));
        return user;
    }

    public static User CreateOAuth(string email, string firstName, string lastName, UserRole role,
        AuthProvider provider, string externalId)
    {
        var user = new User
        {
            Email = email.ToLowerInvariant(),
            PasswordHash = string.Empty,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            Provider = provider,
            ExternalId = externalId,
            IsEmailVerified = true
        };
        user.AddDomainEvent(new Events.UserCreatedDomainEvent(user.Id, user.Email, user.Role.ToString()));
        return user;
    }

    public void AddRefreshToken(RefreshToken refreshToken) => _refreshTokens.Add(refreshToken);

    public void RevokeRefreshToken(string token, string reason)
    {
        var rt = _refreshTokens.FirstOrDefault(r => r.Token == token)
            ?? throw new KeyNotFoundException("Refresh token not found.");
        rt.Revoke(reason);
    }

    public void RecordLogin() => LastLoginAt = DateTime.UtcNow;
    public void Deactivate() { IsActive = false; UpdateTimestamp(); }
    public void VerifyEmail() { IsEmailVerified = true; UpdateTimestamp(); }
}
