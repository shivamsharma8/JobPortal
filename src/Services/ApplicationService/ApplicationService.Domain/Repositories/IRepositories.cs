using ApplicationService.Domain.Entities;

namespace ApplicationService.Domain.Repositories;

public interface IApplicationRepository
{
    Task<Application?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Application?> GetByJobAndCandidateAsync(Guid jobId, Guid candidateId, CancellationToken ct = default);
    Task<IReadOnlyList<Application>> GetByCandidateAsync(Guid candidateId, CancellationToken ct = default);
    Task<IReadOnlyList<Application>> GetByJobAsync(Guid jobId, CancellationToken ct = default);
    Task AddAsync(Application application, CancellationToken ct = default);
    Task UpdateAsync(Application application, CancellationToken ct = default);
    Task<bool> ExistsByJobAndCandidateAsync(Guid jobId, Guid candidateId, CancellationToken ct = default);
}

public interface IApplicationUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
