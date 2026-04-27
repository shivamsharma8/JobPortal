using BuildingBlocks.Common.Pagination;
using JobService.Domain.Entities;
using JobService.Domain.Enums;

namespace JobService.Domain.Repositories;

public interface IJobRepository
{
    Task<Job?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Job> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, string? location, Guid? categoryId,
        JobType? jobType, ExperienceLevel? level, JobStatus? status, CancellationToken ct = default);
    Task AddAsync(Job job, CancellationToken ct = default);
    Task UpdateAsync(Job job, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default);
}

public interface IJobCategoryRepository
{
    Task<IReadOnlyList<JobCategory>> GetAllAsync(CancellationToken ct = default);
    Task<JobCategory?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(JobCategory category, CancellationToken ct = default);
}

public interface IJobUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
