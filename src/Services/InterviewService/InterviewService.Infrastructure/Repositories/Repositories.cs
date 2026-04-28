using InterviewService.Domain.Entities;
using InterviewService.Domain.Repositories;
using InterviewService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InterviewService.Infrastructure.Repositories;

public class InterviewRepository(InterviewDbContext context) : IInterviewRepository
{
    public Task<Interview?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => context.Interviews.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Interview?> GetByApplicationIdAsync(Guid applicationId, CancellationToken ct = default)
        => context.Interviews.FirstOrDefaultAsync(x => x.ApplicationId == applicationId, ct);

    public async Task<IReadOnlyList<Interview>> GetByCandidateAsync(Guid candidateId, CancellationToken ct = default)
        => await context.Interviews.Where(x => x.CandidateId == candidateId)
            .OrderBy(x => x.ScheduledAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Interview>> GetByRecruiterAsync(Guid recruiterId, CancellationToken ct = default)
        => await context.Interviews.Where(x => x.RecruiterId == recruiterId)
            .OrderBy(x => x.ScheduledAt)
            .ToListAsync(ct);

    public async Task AddAsync(Interview interview, CancellationToken ct = default)
        => await context.Interviews.AddAsync(interview, ct);

    public Task UpdateAsync(Interview interview, CancellationToken ct = default)
    {
        context.Interviews.Update(interview);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByApplicationIdAsync(Guid applicationId, CancellationToken ct = default)
        => context.Interviews.AnyAsync(x => x.ApplicationId == applicationId, ct);
}

public class InterviewUnitOfWork(InterviewDbContext context) : IInterviewUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => context.SaveChangesAsync(ct);
}
