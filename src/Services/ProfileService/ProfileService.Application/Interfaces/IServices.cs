namespace ProfileService.Application.Interfaces;

public interface IStorageService
{
    /// <summary>Uploads a file and returns the storage key (e.g. S3 object key).</summary>
    Task<string> UploadAsync(string key, Stream stream, string contentType, CancellationToken ct = default);

    /// <summary>Generates a pre-signed URL for the given storage key.</summary>
    Task<string> GetPresignedUrlAsync(string key, TimeSpan expiry, CancellationToken ct = default);

    /// <summary>Deletes a stored object by key.</summary>
    Task DeleteAsync(string key, CancellationToken ct = default);
}

public interface IProfileCacheService
{
    Task<string?> GetAsync(string key, CancellationToken ct = default);
    Task SetAsync(string key, string value, TimeSpan expiry, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
}
