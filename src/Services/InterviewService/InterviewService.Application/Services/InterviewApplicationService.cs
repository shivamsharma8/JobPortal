using ApplicationService = InterviewService.Domain.Entities.Interview;
using BuildingBlocks.Common.Results;
using InterviewService.Application.DTOs;
using InterviewService.Application.Interfaces;
using InterviewService.Domain.Enums;
using InterviewService.Domain.Repositories;

namespace InterviewService.Application.Services;

public class InterviewApplicationService(
    IInterviewRepository interviewRepository,
    IInterviewUnitOfWork unitOfWork,
    IApplicationServiceClient applicationServiceClient,
    IEventPublisher eventPublisher)
{
    public async Task<Result<InterviewResponse>> ScheduleInterviewAsync(Guid recruiterId, ScheduleInterviewRequest request, CancellationToken ct = default)
    {
        var application = await applicationServiceClient.GetApplicationAsync(request.ApplicationId, ct);
        if (application is null) return Result.Failure<InterviewResponse>(Error.NotFound);
        if (application.RecruiterId != recruiterId) return Result.Failure<InterviewResponse>(Error.Forbidden);
        if (!string.Equals(application.Status, "Shortlisted", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<InterviewResponse>(Error.Custom("Interview.InvalidApplicationState", "Only shortlisted applications can be scheduled."));

        if (await interviewRepository.ExistsByApplicationIdAsync(request.ApplicationId, ct))
            return Result.Failure<InterviewResponse>(Error.Conflict);

        if (!Enum.TryParse<InterviewMode>(request.Mode, true, out var mode))
            return Result.Failure<InterviewResponse>(Error.Custom("Interview.InvalidMode", "Invalid interview mode."));

        var interview = ApplicationService.Create(
            request.ApplicationId,
            application.CandidateId,
            recruiterId,
            request.ScheduledAt,
            mode,
            request.MeetingLink?.Trim() ?? string.Empty,
            request.Location?.Trim() ?? string.Empty,
            request.Notes?.Trim() ?? string.Empty);

        await interviewRepository.AddAsync(interview, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await eventPublisher.PublishAsync(new BuildingBlocks.Contracts.Events.InterviewScheduledEvent
        {
            InterviewId = interview.Id,
            ApplicationId = interview.ApplicationId,
            CandidateId = interview.CandidateId,
            RecruiterId = interview.RecruiterId,
            ScheduledAt = interview.ScheduledAt,
            Mode = interview.Mode.ToString()
        }, ct);

        return Result.Success(Map(interview));
    }

    public async Task<Result<IReadOnlyList<InterviewResponse>>> GetMyInterviewsAsync(Guid userId, string role, CancellationToken ct = default)
    {
        var interviews = role.Equals("Recruiter", StringComparison.OrdinalIgnoreCase)
            ? await interviewRepository.GetByRecruiterAsync(userId, ct)
            : await interviewRepository.GetByCandidateAsync(userId, ct);

        return Result.Success(interviews.Select(Map).ToList() as IReadOnlyList<InterviewResponse>);
    }

    public async Task<Result<InterviewResponse>> ConfirmInterviewAsync(Guid interviewId, Guid candidateId, CancellationToken ct = default)
    {
        var interview = await interviewRepository.GetByIdAsync(interviewId, ct);
        if (interview is null) return Result.Failure<InterviewResponse>(Error.NotFound);
        if (interview.CandidateId != candidateId) return Result.Failure<InterviewResponse>(Error.Forbidden);

        var oldStatus = interview.Status;
        interview.Confirm();
        await interviewRepository.UpdateAsync(interview, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await eventPublisher.PublishAsync(new BuildingBlocks.Contracts.Events.InterviewUpdatedEvent
        {
            InterviewId = interview.Id,
            ApplicationId = interview.ApplicationId,
            CandidateId = interview.CandidateId,
            RecruiterId = interview.RecruiterId,
            OldStatus = oldStatus.ToString(),
            NewStatus = interview.Status.ToString()
        }, ct);

        return Result.Success(Map(interview));
    }

    public async Task<Result<InterviewResponse>> RequestRescheduleAsync(Guid interviewId, Guid candidateId, RequestRescheduleInterviewRequest request, CancellationToken ct = default)
    {
        var interview = await interviewRepository.GetByIdAsync(interviewId, ct);
        if (interview is null) return Result.Failure<InterviewResponse>(Error.NotFound);
        if (interview.CandidateId != candidateId) return Result.Failure<InterviewResponse>(Error.Forbidden);

        var oldStatus = interview.Status;
        interview.RequestReschedule(request.Notes?.Trim() ?? string.Empty);
        await interviewRepository.UpdateAsync(interview, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await eventPublisher.PublishAsync(new BuildingBlocks.Contracts.Events.InterviewUpdatedEvent
        {
            InterviewId = interview.Id,
            ApplicationId = interview.ApplicationId,
            CandidateId = interview.CandidateId,
            RecruiterId = interview.RecruiterId,
            OldStatus = oldStatus.ToString(),
            NewStatus = interview.Status.ToString()
        }, ct);

        return Result.Success(Map(interview));
    }

    public async Task<Result<InterviewResponse>> CancelInterviewAsync(Guid interviewId, Guid recruiterId, CancellationToken ct = default)
    {
        var interview = await interviewRepository.GetByIdAsync(interviewId, ct);
        if (interview is null) return Result.Failure<InterviewResponse>(Error.NotFound);
        if (interview.RecruiterId != recruiterId) return Result.Failure<InterviewResponse>(Error.Forbidden);

        var oldStatus = interview.Status;
        interview.Cancel();
        await interviewRepository.UpdateAsync(interview, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await eventPublisher.PublishAsync(new BuildingBlocks.Contracts.Events.InterviewUpdatedEvent
        {
            InterviewId = interview.Id,
            ApplicationId = interview.ApplicationId,
            CandidateId = interview.CandidateId,
            RecruiterId = interview.RecruiterId,
            OldStatus = oldStatus.ToString(),
            NewStatus = interview.Status.ToString()
        }, ct);

        return Result.Success(Map(interview));
    }

    private static InterviewResponse Map(ApplicationService interview) => new(
        interview.Id,
        interview.ApplicationId,
        interview.CandidateId,
        interview.RecruiterId,
        interview.ScheduledAt,
        interview.Mode.ToString(),
        interview.MeetingLink,
        interview.Location,
        interview.Notes,
        interview.Status.ToString(),
        interview.UpdatedAt);
}
