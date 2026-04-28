using ApplicationService.Domain.Repositories;
using ApplicationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ApplicationEntity = ApplicationService.Domain.Entities.Application;

namespace ApplicationService.Infrastructure.Repositories;

public class ApplicationRepository(ApplicationDbContext context) : IApplicationRepository
{
    public Task<ApplicationEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => context.Applications.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<ApplicationEntity?> GetByJobAndCandidateAsync(Guid jobId, Guid candidateId, CancellationToken ct = default)
        => context.Applications.FirstOrDefaultAsync(x => x.JobId == jobId && x.CandidateId == candidateId, ct);

    public async Task<IReadOnlyList<ApplicationEntity>> GetByCandidateAsync(Guid candidateId, CancellationToken ct = default)
        => await context.Applications.Where(x => x.CandidateId == candidateId)
            .OrderByDescending(x => x.AppliedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ApplicationEntity>> GetByJobAsync(Guid jobId, CancellationToken ct = default)
        => await context.Applications.Where(x => x.JobId == jobId)
            .OrderByDescending(x => x.AppliedAt)
            .ToListAsync(ct);

    public async Task AddAsync(ApplicationEntity application, CancellationToken ct = default)
        => await context.Applications.AddAsync(application, ct);

    public Task UpdateAsync(ApplicationEntity application, CancellationToken ct = default)
    {
        context.Applications.Update(application);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByJobAndCandidateAsync(Guid jobId, Guid candidateId, CancellationToken ct = default)
        => context.Applications.AnyAsync(x => x.JobId == jobId && x.CandidateId == candidateId, ct);
}

public class ApplicationUnitOfWork(ApplicationDbContext context) : IApplicationUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => context.SaveChangesAsync(ct);
}
