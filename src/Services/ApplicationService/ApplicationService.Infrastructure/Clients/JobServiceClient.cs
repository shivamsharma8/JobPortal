using System.Net.Http.Json;
using ApplicationService.Application.DTOs;
using ApplicationService.Application.Interfaces;

namespace ApplicationService.Infrastructure.Clients;

public class JobServiceClient(HttpClient httpClient) : IJobServiceClient
{
    public async Task<JobLookupResponse?> GetJobAsync(Guid jobId, CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"api/v1/jobs/{jobId}", ct);
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<JobLookupResponse>(cancellationToken: ct);
    }
}
