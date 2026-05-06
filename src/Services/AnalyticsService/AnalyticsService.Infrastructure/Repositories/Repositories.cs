using Microsoft.EntityFrameworkCore;
using AnalyticsService.Domain.Entities;
using AnalyticsService.Domain.Repositories;
using AnalyticsService.Infrastructure.Data;

namespace AnalyticsService.Infrastructure.Repositories;

public class JobStatRepository(AnalyticsDbContext context) : IJobStatRepository
{
    public Task<JobStat?> GetByJobIdAsync(Guid jobId, CancellationToken ct = default)
        => context.JobStats.FirstOrDefaultAsync(x => x.JobId == jobId, ct);

    public async Task<IReadOnlyList<JobStat>> GetByRecruiterAsync(Guid recruiterId, CancellationToken ct = default)
        => await context.JobStats
            .Where(x => x.RecruiterId == recruiterId)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(JobStat stat, CancellationToken ct = default)
        => await context.JobStats.AddAsync(stat, ct);

    public Task UpdateAsync(JobStat stat, CancellationToken ct = default)
    {
        context.JobStats.Update(stat);
        return Task.CompletedTask;
    }
}

public class PlatformStatRepository(AnalyticsDbContext context) : IPlatformStatRepository
{
    public Task<PlatformStat?> GetByDateAsync(DateOnly date, CancellationToken ct = default)
        => context.PlatformStats.FirstOrDefaultAsync(x => x.Date == date, ct);

    public async Task<IReadOnlyList<PlatformStat>> GetRecentAsync(int days, CancellationToken ct = default)
    {
        var cutoff = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-days));
        return await context.PlatformStats
            .Where(x => x.Date >= cutoff)
            .OrderByDescending(x => x.Date)
            .ToListAsync(ct);
    }

    public async Task AddAsync(PlatformStat stat, CancellationToken ct = default)
        => await context.PlatformStats.AddAsync(stat, ct);

    public Task UpdateAsync(PlatformStat stat, CancellationToken ct = default)
    {
        context.PlatformStats.Update(stat);
        return Task.CompletedTask;
    }
}

public class AnalyticsUnitOfWork(AnalyticsDbContext context) : IAnalyticsUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => context.SaveChangesAsync(ct);
}
