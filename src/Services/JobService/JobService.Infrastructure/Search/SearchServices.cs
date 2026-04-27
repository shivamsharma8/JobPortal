using JobService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobService.Infrastructure.Search;

/// <summary>
/// Elasticsearch-ready stub. Replace with NEST / Elastic.Clients.Elasticsearch in production.
/// The interface remains unchanged so no application code changes are needed.
/// </summary>
public class ElasticsearchJobSearchService(ILogger<ElasticsearchJobSearchService> logger) : IJobSearchService
{
    public Task IndexJobAsync(Guid jobId, object document, CancellationToken ct = default)
    {
        logger.LogInformation("ES Index (stub) job {JobId}", jobId);
        // TODO: inject IElasticClient and call IndexDocumentAsync(document)
        return Task.CompletedTask;
    }

    public Task RemoveJobAsync(Guid jobId, CancellationToken ct = default)
    {
        logger.LogInformation("ES Remove (stub) job {JobId}", jobId);
        // TODO: inject IElasticClient and call DeleteAsync<Job>(jobId)
        return Task.CompletedTask;
    }
}

public class JobEventPublisher(ILogger<JobEventPublisher> logger) : IJobEventPublisher
{
    public Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class
    {
        logger.LogInformation("Publishing job event {EventType}: {@Event}", typeof(T).Name, @event);
        // TODO: inject IPublishEndpoint from MassTransit and call PublishAsync
        return Task.CompletedTask;
    }
}
