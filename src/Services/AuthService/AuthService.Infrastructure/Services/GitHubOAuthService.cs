using System.Net.Http.Headers;
using System.Text.Json;
using AuthService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Services;

public class GitHubOAuthService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<GitHubOAuthService> logger) : IGitHubOAuthService
{
    public async Task<GitHubUserInfo?> GetUserInfoAsync(string code, CancellationToken ct = default)
    {
        try
        {
            var clientId = configuration["GitHub:ClientId"]!;
            var clientSecret = configuration["GitHub:ClientSecret"]!;

            var client = httpClientFactory.CreateClient("GitHub");

            // Exchange code for access token
            var tokenResponse = await client.PostAsync(
                "https://github.com/login/oauth/access_token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["code"] = code
                }), ct);

            var tokenContent = await tokenResponse.Content.ReadAsStringAsync(ct);
            var accessToken = ParseAccessToken(tokenContent);
            if (string.IsNullOrEmpty(accessToken)) return null;

            // Fetch user profile
            var userRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
            userRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            userRequest.Headers.UserAgent.Add(new ProductInfoHeaderValue("HireConnect", "1.0"));

            var userResponse = await client.SendAsync(userRequest, ct);
            if (!userResponse.IsSuccessStatusCode) return null;

            var userJson = await userResponse.Content.ReadAsStringAsync(ct);
            var userDoc = JsonDocument.Parse(userJson);
            var root = userDoc.RootElement;

            var email = root.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
            if (string.IsNullOrEmpty(email))
                email = await GetPrimaryEmailAsync(client, accessToken, ct);

            if (string.IsNullOrEmpty(email)) return null;

            return new GitHubUserInfo(
                root.GetProperty("id").GetInt64().ToString(),
                email,
                root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? email : email,
                root.GetProperty("login").GetString() ?? string.Empty
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GitHub OAuth failed");
            return null;
        }
    }

    private static string? ParseAccessToken(string content)
    {
        var parts = content.Split('&');
        foreach (var part in parts)
        {
            var kv = part.Split('=');
            if (kv.Length == 2 && kv[0] == "access_token")
                return Uri.UnescapeDataString(kv[1]);
        }
        return null;
    }

    private static async Task<string?> GetPrimaryEmailAsync(HttpClient client, string accessToken, CancellationToken ct)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        req.Headers.UserAgent.Add(new ProductInfoHeaderValue("HireConnect", "1.0"));

        var res = await client.SendAsync(req, ct);
        if (!res.IsSuccessStatusCode) return null;

        var json = await res.Content.ReadAsStringAsync(ct);
        var doc = JsonDocument.Parse(json);
        foreach (var item in doc.RootElement.EnumerateArray())
        {
            if (item.TryGetProperty("primary", out var primary) && primary.GetBoolean()
                && item.TryGetProperty("email", out var emailProp))
                return emailProp.GetString();
        }
        return null;
    }
}
