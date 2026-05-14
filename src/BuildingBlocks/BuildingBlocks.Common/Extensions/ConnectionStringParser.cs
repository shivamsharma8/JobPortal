namespace BuildingBlocks.Common.Extensions;

public static class ConnectionStringParser
{
    public static string ParseUrlToNpgsql(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        if (!url.StartsWith("postgres://") && !url.StartsWith("postgresql://"))
            return url;

        var uri = new Uri(url);
        var userInfo = uri.UserInfo.Split(':');
        
        return $"Host={uri.Host};Port={(uri.Port > 0 ? uri.Port : 5432)};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={(userInfo.Length > 1 ? userInfo[1] : "")};SslMode=Prefer;";
    }
}
