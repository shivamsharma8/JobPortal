using BuildingBlocks.Common.Domain;
using InterviewService.Domain.Enums;

namespace InterviewService.Domain.Entities;

public sealed class Interview : BaseEntity
{
    private Interview() { }

    public Guid ApplicationId { get; private set; }
    public Guid CandidateId { get; private set; }
    public Guid RecruiterId { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public InterviewMode Mode { get; private set; }
    public string MeetingLink { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public InterviewStatus Status { get; private set; }

    public static Interview Create(
        Guid applicationId,
        Guid candidateId,
        Guid recruiterId,
        DateTime scheduledAt,
        InterviewMode mode,
        string meetingLink,
        string location,
        string notes)
    {
        if (scheduledAt <= DateTime.UtcNow)
            throw new InvalidOperationException("Interview time must be in the future.");

        return new Interview
        {
            ApplicationId = applicationId,
            CandidateId = candidateId,
            RecruiterId = recruiterId,
            ScheduledAt = scheduledAt,
            Mode = mode,
            MeetingLink = meetingLink,
            Location = location,
            Notes = notes,
            Status = InterviewStatus.Scheduled
        };
    }

    public void Confirm()
    {
        if (Status != InterviewStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled interviews can be confirmed.");

        Status = InterviewStatus.Confirmed;
        UpdateTimestamp();
    }

    public void RequestReschedule(string notes)
    {
        if (Status is InterviewStatus.Cancelled or InterviewStatus.Completed)
            throw new InvalidOperationException("Closed interviews cannot be rescheduled.");

        Notes = notes;
        Status = InterviewStatus.RescheduleRequested;
        UpdateTimestamp();
    }

    public void Cancel()
    {
        if (Status is InterviewStatus.Cancelled or InterviewStatus.Completed)
            throw new InvalidOperationException("Interview is already closed.");

        Status = InterviewStatus.Cancelled;
        UpdateTimestamp();
    }

    public void Complete()
    {
        if (Status is InterviewStatus.Cancelled or InterviewStatus.Completed)
            throw new InvalidOperationException("Interview is already closed.");

        Status = InterviewStatus.Completed;
        UpdateTimestamp();
    }
}
