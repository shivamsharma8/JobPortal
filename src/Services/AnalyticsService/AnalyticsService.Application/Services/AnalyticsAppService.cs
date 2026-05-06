using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using AnalyticsService.Application.DTOs;
using AnalyticsService.Domain.Entities;
using AnalyticsService.Domain.Repositories;
using BuildingBlocks.Common.Results;

namespace AnalyticsService.Application.Services;

public class AnalyticsAppService(
    IJobStatRepository jobStatRepository,
    IPlatformStatRepository platformStatRepository,
    IAnalyticsUnitOfWork unitOfWork,
    Microsoft.Extensions.Caching.Distributed.IDistributedCache cache)
{
    public async Task<Result<JobStatResponse>> GetJobStatsAsync(
        Guid jobId, Guid recruiterId, CancellationToken ct = default)
    {
        var stat = await jobStatRepository.GetByJobIdAsync(jobId, ct);
        if (stat is null)
            return Result.Failure<JobStatResponse>(Error.NotFound);

        if (stat.RecruiterId != recruiterId)
            return Result.Failure<JobStatResponse>(Error.Forbidden);

        return Result.Success(MapJobStat(stat));
    }

    public async Task<Result<RecruiterDashboardResponse>> GetRecruiterDashboardAsync(
        Guid recruiterId, CancellationToken ct = default)
    {
        var stats = await jobStatRepository.GetByRecruiterAsync(recruiterId, ct);

        var dashboard = new RecruiterDashboardResponse(
            RecruiterId: recruiterId,
            TotalJobs: stats.Count,
            TotalApplications: stats.Sum(s => s.TotalApplications),
            TotalShortlisted: stats.Sum(s => s.Shortlisted),
            TotalInterviews: stats.Sum(s => s.InterviewsScheduled),
            TotalOffers: stats.Sum(s => s.Offered),
            TotalRejected: stats.Sum(s => s.Rejected));

        return Result.Success(dashboard);
    }

    public async Task<Result<IReadOnlyList<PlatformStatResponse>>> GetPlatformStatsAsync(
        int days, CancellationToken ct = default)
    {
        var cacheKey = $"platform_stats_{days}";
        var cachedData = await cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return Result.Success(JsonSerializer.Deserialize<IReadOnlyList<PlatformStatResponse>>(cachedData)!);
        }

        var stats = await platformStatRepository.GetRecentAsync(days, ct);
        var response = stats.Select(MapPlatform).ToList() as IReadOnlyList<PlatformStatResponse>;

        await cache.SetStringAsync(
            cacheKey, 
            JsonSerializer.Serialize(response), 
            new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) }, 
            ct);

        return Result.Success(response);
    }

    // ── Called by the consumer worker ────────────────────────────────────────

    public async Task HandleApplicationSubmittedAsync(
        Guid jobId, Guid recruiterId, CancellationToken ct = default)
    {
        var stat = await GetOrCreateJobStatAsync(jobId, recruiterId, ct);
        stat.IncrementApplications();
        await jobStatRepository.UpdateAsync(stat, ct);

        var platform = await GetOrCreateTodayPlatformStatAsync(ct);
        platform.IncrementApplications();
        await platformStatRepository.UpdateAsync(platform, ct);

        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task HandleStatusChangedAsync(
        Guid jobId, Guid recruiterId, string newStatus, CancellationToken ct = default)
    {
        var stat = await GetOrCreateJobStatAsync(jobId, recruiterId, ct);
        stat.RecordStatusChange(newStatus);
        await jobStatRepository.UpdateAsync(stat, ct);

        if (newStatus == "Offered")
        {
            var platform = await GetOrCreateTodayPlatformStatAsync(ct);
            platform.IncrementOffers();
            await platformStatRepository.UpdateAsync(platform, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task HandleJobPostedAsync(CancellationToken ct = default)
    {
        var platform = await GetOrCreateTodayPlatformStatAsync(ct);
        platform.IncrementJobs();
        await platformStatRepository.UpdateAsync(platform, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task HandleInterviewScheduledAsync(CancellationToken ct = default)
    {
        var platform = await GetOrCreateTodayPlatformStatAsync(ct);
        platform.IncrementInterviews();
        await platformStatRepository.UpdateAsync(platform, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<JobStat> GetOrCreateJobStatAsync(
        Guid jobId, Guid recruiterId, CancellationToken ct)
    {
        var stat = await jobStatRepository.GetByJobIdAsync(jobId, ct);
        if (stat is not null) return stat;

        stat = JobStat.Create(jobId, recruiterId);
        await jobStatRepository.AddAsync(stat, ct);
        return stat;
    }

    private async Task<PlatformStat> GetOrCreateTodayPlatformStatAsync(CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var stat = await platformStatRepository.GetByDateAsync(today, ct);
        if (stat is not null) return stat;

        stat = PlatformStat.CreateForToday();
        await platformStatRepository.AddAsync(stat, ct);
        return stat;
    }

    private static JobStatResponse MapJobStat(JobStat s) => new(
        s.JobId, s.RecruiterId,
        s.TotalApplications, s.Shortlisted, s.InterviewsScheduled,
        s.Offered, s.Rejected, s.Withdrawn,
        s.FirstApplicationAt, s.LatestApplicationAt, s.UpdatedAt);

    private static PlatformStatResponse MapPlatform(PlatformStat s) => new(
        s.Date, s.TotalJobs, s.TotalApplications, s.TotalInterviews, s.TotalOffers);
}
