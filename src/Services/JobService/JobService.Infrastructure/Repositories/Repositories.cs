using JobService.Domain.Entities;
using JobService.Domain.Enums;
using JobService.Domain.Repositories;
using JobService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobService.Infrastructure.Repositories;

public class JobRepository(JobDbContext context) : IJobRepository
{
    public async Task<Job?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Jobs.Include(j => j.Skills).FirstOrDefaultAsync(j => j.Id == id, ct);

    public async Task<(IReadOnlyList<Job> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, string? location, Guid? categoryId,
        JobType? jobType, ExperienceLevel? level, JobStatus? status, CancellationToken ct = default)
    {
        var query = context.Jobs.Include(j => j.Skills).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(j => EF.Functions.ILike(j.Title, $"%{search}%")
                               || EF.Functions.ILike(j.Description, $"%{search}%")
                               || EF.Functions.ILike(j.Company, $"%{search}%"));

        if (!string.IsNullOrWhiteSpace(location))
            query = query.Where(j => EF.Functions.ILike(j.Location, $"%{location}%") || j.IsRemote);

        if (categoryId.HasValue)
            query = query.Where(j => j.CategoryId == categoryId.Value);

        if (jobType.HasValue)
            query = query.Where(j => j.JobType == jobType.Value);

        if (level.HasValue)
            query = query.Where(j => j.ExperienceLevel == level.Value);

        if (status.HasValue)
            query = query.Where(j => j.Status == status.Value);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(j => j.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task AddAsync(Job job, CancellationToken ct = default)
        => await context.Jobs.AddAsync(job, ct);

    public Task UpdateAsync(Job job, CancellationToken ct = default)
    {
        context.Jobs.Update(job);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var job = await context.Jobs.FindAsync([id], ct);
        if (job is not null) context.Jobs.Remove(job);
    }

    public Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default)
        => context.Jobs.AnyAsync(j => j.Id == id, ct);
}

public class JobCategoryRepository(JobDbContext context) : IJobCategoryRepository
{
    public async Task<IReadOnlyList<JobCategory>> GetAllAsync(CancellationToken ct = default)
        => await context.JobCategories.ToListAsync(ct);

    public async Task<JobCategory?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.JobCategories.FindAsync([id], ct);

    public async Task AddAsync(JobCategory category, CancellationToken ct = default)
        => await context.JobCategories.AddAsync(category, ct);
}

public class JobUnitOfWork(JobDbContext context) : IJobUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}
