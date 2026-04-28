using BuildingBlocks.Common.Domain;
using ApplicationService.Domain.Enums;

namespace ApplicationService.Domain.Entities;

public sealed class Application : BaseEntity
{
    private Application() { }

    public Guid JobId { get; private set; }
    public Guid CandidateId { get; private set; }
    public Guid RecruiterId { get; private set; }
    public string ResumeUrl { get; private set; } = string.Empty;
    public string CoverLetter { get; private set; } = string.Empty;
    public ApplicationStatus Status { get; private set; }
    public DateTime AppliedAt { get; private set; }

    public static Application Create(Guid jobId, Guid candidateId, Guid recruiterId, string resumeUrl, string coverLetter)
    {
        return new Application
        {
            JobId = jobId,
            CandidateId = candidateId,
            RecruiterId = recruiterId,
            ResumeUrl = resumeUrl,
            CoverLetter = coverLetter,
            Status = ApplicationStatus.Applied,
            AppliedAt = DateTime.UtcNow
        };
    }

    public void UpdateStatus(ApplicationStatus status)
    {
        if (Status is ApplicationStatus.Rejected or ApplicationStatus.Withdrawn)
            throw new InvalidOperationException("Closed applications cannot be updated.");

        if (status is ApplicationStatus.Applied or ApplicationStatus.Withdrawn)
            throw new InvalidOperationException("Use the submit or withdraw flow for these statuses.");

        if (!IsValidTransition(Status, status))
            throw new InvalidOperationException($"Cannot move application from {Status} to {status}.");

        Status = status;
        UpdateTimestamp();
    }

    public void Withdraw()
    {
        if (Status is ApplicationStatus.Rejected or ApplicationStatus.Withdrawn)
            throw new InvalidOperationException("This application is already closed.");

        Status = ApplicationStatus.Withdrawn;
        UpdateTimestamp();
    }

    private static bool IsValidTransition(ApplicationStatus current, ApplicationStatus next) => current switch
    {
        ApplicationStatus.Applied => next is ApplicationStatus.Shortlisted,
        ApplicationStatus.Shortlisted => next is ApplicationStatus.InterviewScheduled or ApplicationStatus.Offered or ApplicationStatus.Rejected,
        ApplicationStatus.InterviewScheduled => next is ApplicationStatus.Offered or ApplicationStatus.Rejected,
        ApplicationStatus.Offered => next is ApplicationStatus.Rejected,
        _ => false
    };
}
