namespace JobService.Application.Interfaces;

public interface IJobSearchService
{
    /// <summary>
    /// Elasticsearch-ready abstraction. Index a job document for full-text search.
    /// In development this is a no-op; wire up NEST/Elastic.Clients.Elasticsearch in production.
    /// </summary>
    Task IndexJobAsync(Guid jobId, object document, CancellationToken ct = default);

    /// <summary>Remove a job from the search index.</summary>
    Task RemoveJobAsync(Guid jobId, CancellationToken ct = default);
}

public interface IJobEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class;
}
