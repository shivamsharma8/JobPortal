namespace AuthService.Application.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface IGitHubOAuthService
{
    Task<GitHubUserInfo?> GetUserInfoAsync(string code, CancellationToken ct = default);
}

public record GitHubUserInfo(
    string Id,
    string Email,
    string Name,
    string Login
);

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class;
}
