using ProfileService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ProfileService.Infrastructure.Storage;

/// <summary>
/// Local filesystem storage implementation — swap for AWSSDK.S3 in production.
/// Implements the same IStorageService interface so no application code changes are needed.
/// </summary>
public class LocalStorageService(IConfiguration config, ILogger<LocalStorageService> logger) : IStorageService
{
    private readonly string _basePath = config["Storage:LocalBasePath"] ?? Path.Combine(Path.GetTempPath(), "hireconnect-storage");

    public async Task<string> UploadAsync(string key, Stream stream, string contentType, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, key.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await using var fs = File.Create(fullPath);
        await stream.CopyToAsync(fs, ct);
        logger.LogInformation("Stored file at {Path}", fullPath);
        return key;
    }

    public Task<string> GetPresignedUrlAsync(string key, TimeSpan expiry, CancellationToken ct = default)
    {
        // In production replace with: AmazonS3Client.GetPreSignedURL(...)
        var url = $"/storage/{key}";
        return Task.FromResult(url);
    }

    public Task DeleteAsync(string key, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, key.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Redis-ready cache service — backed by IDistributedCache.
/// Swap MemoryDistributedCache for StackExchangeRedisCache in production.
/// </summary>
public class ProfileCacheService(Microsoft.Extensions.Caching.Distributed.IDistributedCache cache) : IProfileCacheService
{
    public async Task<string?> GetAsync(string key, CancellationToken ct = default)
    {
        var bytes = await cache.GetAsync(key, ct);
        return bytes is null ? null : System.Text.Encoding.UTF8.GetString(bytes);
    }

    public Task SetAsync(string key, string value, TimeSpan expiry, CancellationToken ct = default)
        => cache.SetAsync(key, System.Text.Encoding.UTF8.GetBytes(value),
            new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry }, ct);

    public Task RemoveAsync(string key, CancellationToken ct = default)
        => cache.RemoveAsync(key, ct);
}
