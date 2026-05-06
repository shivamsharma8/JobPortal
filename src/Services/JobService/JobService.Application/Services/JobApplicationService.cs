using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using BuildingBlocks.Common.Pagination;
using BuildingBlocks.Common.Results;
using JobService.Application.DTOs;
using JobService.Application.Interfaces;
using JobService.Domain.Entities;
using JobService.Domain.Enums;
using JobService.Domain.Repositories;

namespace JobService.Application.Services;

public class JobApplicationService(
    IJobRepository jobRepository,
    IJobCategoryRepository categoryRepository,
    IJobUnitOfWork unitOfWork,
    IJobSearchService searchService,
    IJobEventPublisher eventPublisher,
    Microsoft.Extensions.Caching.Distributed.IDistributedCache cache)
{
    public async Task<Result<JobResponse>> CreateJobAsync(
        Guid recruiterId, CreateJobRequest request, CancellationToken ct = default)
    {
        if (!Enum.TryParse<JobType>(request.JobType, true, out var jobType))
            return Result.Failure<JobResponse>(Error.Custom("Job.InvalidType", "Invalid job type."));

        if (!Enum.TryParse<ExperienceLevel>(request.ExperienceLevel, true, out var level))
            return Result.Failure<JobResponse>(Error.Custom("Job.InvalidLevel", "Invalid experience level."));

        if (!await categoryRepository.GetByIdAsync(request.CategoryId, ct).ContinueWith(t => t.Result != null))
            return Result.Failure<JobResponse>(Error.Custom("Job.InvalidCategory", "Category not found."));

        var job = Job.Create(recruiterId, request.Title, request.Description, request.Company,
            request.CompanyLogoUrl, request.Location, request.IsRemote, jobType, level,
            request.SalaryMin, request.SalaryMax, request.Currency, request.CategoryId, request.ExpiresAt);

        foreach (var skill in request.Skills)
            job.AddSkill(JobSkill.Create(job.Id, skill.Name, skill.IsRequired));

        await jobRepository.AddAsync(job, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await searchService.IndexJobAsync(job.Id, MapToResponse(job), ct);
        await eventPublisher.PublishAsync(new BuildingBlocks.Contracts.Events.JobPostedEvent
        {
            JobId = job.Id,
            RecruiterId = job.RecruiterId,
            Title = job.Title,
            Company = job.Company
        }, ct);

        return Result.Success(MapToResponse(job));
    }

    public async Task<Result<JobResponse>> UpdateJobAsync(
        Guid jobId, Guid recruiterId, UpdateJobRequest request, CancellationToken ct = default)
    {
        var job = await jobRepository.GetByIdAsync(jobId, ct);
        if (job is null) return Result.Failure<JobResponse>(Error.NotFound);
        if (job.RecruiterId != recruiterId) return Result.Failure<JobResponse>(Error.Forbidden);

        if (!Enum.TryParse<JobType>(request.JobType, true, out var jobType))
            return Result.Failure<JobResponse>(Error.Custom("Job.InvalidType", "Invalid job type."));

        if (!Enum.TryParse<ExperienceLevel>(request.ExperienceLevel, true, out var level))
            return Result.Failure<JobResponse>(Error.Custom("Job.InvalidLevel", "Invalid experience level."));

        job.Update(request.Title, request.Description, request.Location, request.IsRemote,
            jobType, level, request.SalaryMin, request.SalaryMax, request.Currency,
            request.CategoryId, request.ExpiresAt);

        job.ClearSkills();
        foreach (var skill in request.Skills)
            job.AddSkill(JobSkill.Create(job.Id, skill.Name, skill.IsRequired));

        await jobRepository.UpdateAsync(job, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await searchService.IndexJobAsync(job.Id, MapToResponse(job), ct);

        return Result.Success(MapToResponse(job));
    }

    public async Task<Result> PauseJobAsync(Guid jobId, Guid recruiterId, CancellationToken ct = default)
    {
        var job = await jobRepository.GetByIdAsync(jobId, ct);
        if (job is null) return Result.Failure(Error.NotFound);
        if (job.RecruiterId != recruiterId) return Result.Failure(Error.Forbidden);
        job.Pause();
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteJobAsync(Guid jobId, Guid recruiterId, CancellationToken ct = default)
    {
        var job = await jobRepository.GetByIdAsync(jobId, ct);
        if (job is null) return Result.Failure(Error.NotFound);
        if (job.RecruiterId != recruiterId) return Result.Failure(Error.Forbidden);
        job.Close();
        await jobRepository.DeleteAsync(jobId, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await searchService.RemoveJobAsync(jobId, ct);
        return Result.Success();
    }

    public async Task<Result<JobResponse>> GetJobByIdAsync(Guid jobId, CancellationToken ct = default)
    {
        var job = await jobRepository.GetByIdAsync(jobId, ct);
        if (job is null) return Result.Failure<JobResponse>(Error.NotFound);
        job.IncrementViewCount();
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(MapToResponse(job));
    }

    public async Task<Result<PagedResponse<JobResponse>>> SearchJobsAsync(
        JobSearchRequest request, CancellationToken ct = default)
    {
        var cacheKey = $"search_jobs_{request.Q}_{request.Location}_{request.CategoryId}_{request.JobType}_{request.ExperienceLevel}_{request.PageNumber}_{request.PageSize}";
        var cachedData = await cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return Result.Success(JsonSerializer.Deserialize<PagedResponse<JobResponse>>(cachedData)!);
        }

        JobType? jobType = null;
        ExperienceLevel? level = null;

        if (!string.IsNullOrEmpty(request.JobType) && Enum.TryParse<JobType>(request.JobType, true, out var jt))
            jobType = jt;
        if (!string.IsNullOrEmpty(request.ExperienceLevel) && Enum.TryParse<ExperienceLevel>(request.ExperienceLevel, true, out var lvl))
            level = lvl;

        var (items, totalCount) = await jobRepository.GetPagedAsync(
            request.PageNumber, request.PageSize, request.Q, request.Location,
            request.CategoryId, jobType, level, JobStatus.Active, ct);

        var response = PagedResponse<JobResponse>.Create(
            items.Select(MapToResponse).ToList(),
            totalCount, request.PageNumber, request.PageSize);

        await cache.SetStringAsync(
            cacheKey, 
            JsonSerializer.Serialize(response), 
            new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) }, 
            ct);

        return Result.Success(response);
    }

    private static JobResponse MapToResponse(Job j) => new(
        j.Id, j.RecruiterId, j.Title, j.Description, j.Company, j.CompanyLogoUrl,
        j.Location, j.IsRemote, j.JobType.ToString(), j.ExperienceLevel.ToString(),
        j.SalaryMin, j.SalaryMax, j.Currency, j.Status.ToString(),
        j.PublishedAt, j.ExpiresAt, j.CategoryId, j.ViewCount, j.ApplicationCount,
        j.Skills.Select(s => new JobSkillDto(s.Id, s.Name, s.IsRequired)).ToList(),
        j.CreatedAt);
}
