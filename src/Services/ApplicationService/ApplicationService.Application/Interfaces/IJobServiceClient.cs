using ApplicationService.Application.DTOs;

namespace ApplicationService.Application.Interfaces;

public interface IJobServiceClient
{
    Task<JobLookupResponse?> GetJobAsync(Guid jobId, CancellationToken ct = default);
}
