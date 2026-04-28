using InterviewService.Domain.Entities;

namespace InterviewService.Domain.Repositories;

public interface IInterviewRepository
{
    Task<Interview?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Interview?> GetByApplicationIdAsync(Guid applicationId, CancellationToken ct = default);
    Task<IReadOnlyList<Interview>> GetByCandidateAsync(Guid candidateId, CancellationToken ct = default);
    Task<IReadOnlyList<Interview>> GetByRecruiterAsync(Guid recruiterId, CancellationToken ct = default);
    Task AddAsync(Interview interview, CancellationToken ct = default);
    Task UpdateAsync(Interview interview, CancellationToken ct = default);
    Task<bool> ExistsByApplicationIdAsync(Guid applicationId, CancellationToken ct = default);
}

public interface IInterviewUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
