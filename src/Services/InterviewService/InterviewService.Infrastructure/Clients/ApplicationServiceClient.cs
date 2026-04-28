using System.Net.Http.Json;
using InterviewService.Application.DTOs;
using InterviewService.Application.Interfaces;

namespace InterviewService.Infrastructure.Clients;

public class ApplicationServiceClient(HttpClient httpClient) : IApplicationServiceClient
{
    public async Task<ApplicationLookupResponse?> GetApplicationAsync(Guid applicationId, CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"api/v1/applications/{applicationId}", ct);
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<ApplicationLookupResponse>(cancellationToken: ct);
    }
}
