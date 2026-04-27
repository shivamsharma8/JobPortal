using BuildingBlocks.Common.Domain;

namespace AuthService.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    private RefreshToken() { }

    public string Token { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; } = false;
    public string? RevokedReason { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public string? CreatedByIp { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public static RefreshToken Create(string token, Guid userId, int expiryDays, string? createdByIp = null)
        => new()
        {
            Token = token,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            CreatedByIp = createdByIp
        };

    public void Revoke(string reason, string? replacedByToken = null)
    {
        IsRevoked = true;
        RevokedReason = reason;
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
    }
}
