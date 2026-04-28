using InterviewService.Application.DTOs;

namespace InterviewService.Application.Interfaces;

public interface IApplicationServiceClient
{
    Task<ApplicationLookupResponse?> GetApplicationAsync(Guid applicationId, CancellationToken ct = default);
}
