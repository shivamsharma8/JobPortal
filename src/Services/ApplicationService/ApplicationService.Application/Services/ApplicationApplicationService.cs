using ApplicationService.Application.DTOs;
using ApplicationService.Application.Interfaces;
using ApplicationService.Domain.Enums;
using ApplicationService.Domain.Repositories;
using BuildingBlocks.Common.Results;
using ApplicationEntity = ApplicationService.Domain.Entities.Application;

namespace ApplicationService.Application.Services;

public class ApplicationApplicationService(
    IApplicationRepository applicationRepository,
    IApplicationUnitOfWork unitOfWork,
    IJobServiceClient jobServiceClient,
    IEventPublisher eventPublisher)
{
    public async Task<Result<ApplicationResponse>> SubmitApplicationAsync(
        Guid candidateId,
        SubmitApplicationRequest request,
        CancellationToken ct = default)
    {
        var job = await jobServiceClient.GetJobAsync(request.JobId, ct);
        if (job is null) return Result.Failure<ApplicationResponse>(Error.NotFound);
        if (!string.Equals(job.Status, "Active", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<ApplicationResponse>(Error.Custom("Application.JobClosed", "Job is not accepting applications."));

        if (await applicationRepository.ExistsByJobAndCandidateAsync(request.JobId, candidateId, ct))
            return Result.Failure<ApplicationResponse>(Error.Conflict);

        var application = ApplicationEntity.Create(
            request.JobId,
            candidateId,
            job.RecruiterId,
            request.ResumeUrl?.Trim() ?? string.Empty,
            request.CoverLetter?.Trim() ?? string.Empty);

        await applicationRepository.AddAsync(application, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await eventPublisher.PublishAsync(new BuildingBlocks.Contracts.Events.ApplicationSubmittedEvent
        {
            ApplicationId = application.Id,
            JobId = application.JobId,
            CandidateId = application.CandidateId,
            RecruiterId = application.RecruiterId,
            ResumeUrl = application.ResumeUrl,
            CoverLetter = application.CoverLetter,
            Status = application.Status.ToString()
        }, ct);

        return Result.Success(Map(application));
    }

    public async Task<Result<IReadOnlyList<ApplicationResponse>>> GetMyApplicationsAsync(Guid candidateId, CancellationToken ct = default)
    {
        var applications = await applicationRepository.GetByCandidateAsync(candidateId, ct);
        return Result.Success(applications.Select(Map).ToList() as IReadOnlyList<ApplicationResponse>);
    }

    public async Task<Result<IReadOnlyList<ApplicationResponse>>> GetApplicationsForJobAsync(Guid jobId, Guid recruiterId, CancellationToken ct = default)
    {
        var job = await jobServiceClient.GetJobAsync(jobId, ct);
        if (job is null) return Result.Failure<IReadOnlyList<ApplicationResponse>>(Error.NotFound);
        if (job.RecruiterId != recruiterId) return Result.Failure<IReadOnlyList<ApplicationResponse>>(Error.Forbidden);

        var applications = await applicationRepository.GetByJobAsync(jobId, ct);
        return Result.Success(applications.Select(Map).ToList() as IReadOnlyList<ApplicationResponse>);
    }

    public async Task<Result<ApplicationResponse>> GetApplicationByIdAsync(Guid applicationId, Guid userId, string role, CancellationToken ct = default)
    {
        var application = await applicationRepository.GetByIdAsync(applicationId, ct);
        if (application is null) return Result.Failure<ApplicationResponse>(Error.NotFound);

        if (role.Equals("Candidate", StringComparison.OrdinalIgnoreCase) && application.CandidateId != userId)
            return Result.Failure<ApplicationResponse>(Error.Forbidden);

        if (role.Equals("Recruiter", StringComparison.OrdinalIgnoreCase) && application.RecruiterId != userId)
            return Result.Failure<ApplicationResponse>(Error.Forbidden);

        return Result.Success(Map(application));
    }

    public async Task<Result<ApplicationResponse>> UpdateStatusAsync(
        Guid applicationId,
        Guid recruiterId,
        UpdateApplicationStatusRequest request,
        CancellationToken ct = default)
    {
        var application = await applicationRepository.GetByIdAsync(applicationId, ct);
        if (application is null) return Result.Failure<ApplicationResponse>(Error.NotFound);
        if (application.RecruiterId != recruiterId) return Result.Failure<ApplicationResponse>(Error.Forbidden);

        if (!Enum.TryParse<ApplicationStatus>(request.Status, true, out var nextStatus))
            return Result.Failure<ApplicationResponse>(Error.Custom("Application.InvalidStatus", "Invalid application status."));

        var currentStatus = application.Status;
        application.UpdateStatus(nextStatus);
        await applicationRepository.UpdateAsync(application, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await eventPublisher.PublishAsync(new BuildingBlocks.Contracts.Events.ApplicationStatusChangedEvent
        {
            ApplicationId = application.Id,
            JobId = application.JobId,
            CandidateId = application.CandidateId,
            RecruiterId = application.RecruiterId,
            OldStatus = currentStatus.ToString(),
            NewStatus = application.Status.ToString()
        }, ct);

        return Result.Success(Map(application));
    }

    public async Task<Result<ApplicationResponse>> WithdrawAsync(Guid applicationId, Guid candidateId, CancellationToken ct = default)
    {
        var application = await applicationRepository.GetByIdAsync(applicationId, ct);
        if (application is null) return Result.Failure<ApplicationResponse>(Error.NotFound);
        if (application.CandidateId != candidateId) return Result.Failure<ApplicationResponse>(Error.Forbidden);

        var currentStatus = application.Status;
        application.Withdraw();
        await applicationRepository.UpdateAsync(application, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await eventPublisher.PublishAsync(new BuildingBlocks.Contracts.Events.ApplicationStatusChangedEvent
        {
            ApplicationId = application.Id,
            JobId = application.JobId,
            CandidateId = application.CandidateId,
            RecruiterId = application.RecruiterId,
            OldStatus = currentStatus.ToString(),
            NewStatus = application.Status.ToString()
        }, ct);

        return Result.Success(Map(application));
    }

    private static ApplicationResponse Map(ApplicationEntity application) => new(
        application.Id,
        application.JobId,
        application.CandidateId,
        application.RecruiterId,
        application.ResumeUrl,
        application.CoverLetter,
        application.Status.ToString(),
        application.AppliedAt,
        application.UpdatedAt);
}
