using BuildingBlocks.Common.Domain;

namespace AuthService.Domain.Events;

public record UserCreatedDomainEvent(Guid UserId, string Email, string Role) : DomainEvent
{
    public override string EventType => "UserCreated";
}

public record UserLoggedInDomainEvent(Guid UserId, string Email) : DomainEvent
{
    public override string EventType => "UserLoggedIn";
}

public record RefreshTokenRevokedDomainEvent(Guid UserId, string Token) : DomainEvent
{
    public override string EventType => "RefreshTokenRevoked";
}
