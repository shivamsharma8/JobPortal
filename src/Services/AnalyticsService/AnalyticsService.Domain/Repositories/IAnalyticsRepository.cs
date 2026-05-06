using AnalyticsService.Domain.Entities;

namespace AnalyticsService.Domain.Repositories;

public interface IJobStatRepository
{
    Task<JobStat?> GetByJobIdAsync(Guid jobId, CancellationToken ct = default);
    Task<IReadOnlyList<JobStat>> GetByRecruiterAsync(Guid recruiterId, CancellationToken ct = default);
    Task AddAsync(JobStat stat, CancellationToken ct = default);
    Task UpdateAsync(JobStat stat, CancellationToken ct = default);
}

public interface IPlatformStatRepository
{
    Task<PlatformStat?> GetByDateAsync(DateOnly date, CancellationToken ct = default);
    Task<IReadOnlyList<PlatformStat>> GetRecentAsync(int days, CancellationToken ct = default);
    Task AddAsync(PlatformStat stat, CancellationToken ct = default);
    Task UpdateAsync(PlatformStat stat, CancellationToken ct = default);
}

public interface IAnalyticsUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
