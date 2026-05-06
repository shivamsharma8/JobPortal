using BuildingBlocks.Common.Domain;

namespace AnalyticsService.Domain.Entities;

/// <summary>
/// Per-job application pipeline statistics.
/// Upserted whenever a relevant event is consumed.
/// </summary>
public sealed class JobStat : BaseEntity
{
    private JobStat() { }

    public Guid JobId { get; private set; }
    public Guid RecruiterId { get; private set; }
    public int TotalApplications { get; private set; }
    public int Shortlisted { get; private set; }
    public int InterviewsScheduled { get; private set; }
    public int Offered { get; private set; }
    public int Rejected { get; private set; }
    public int Withdrawn { get; private set; }
    public DateTime? FirstApplicationAt { get; private set; }
    public DateTime? LatestApplicationAt { get; private set; }

    public static JobStat Create(Guid jobId, Guid recruiterId)
    {
        return new JobStat
        {
            JobId = jobId,
            RecruiterId = recruiterId
        };
    }

    public void IncrementApplications()
    {
        TotalApplications++;
        var now = DateTime.UtcNow;
        FirstApplicationAt ??= now;
        LatestApplicationAt = now;
        UpdateTimestamp();
    }

    public void RecordStatusChange(string newStatus)
    {
        switch (newStatus)
        {
            case "Shortlisted": Shortlisted++; break;
            case "InterviewScheduled": InterviewsScheduled++; break;
            case "Offered": Offered++; break;
            case "Rejected": Rejected++; break;
            case "Withdrawn": Withdrawn++; break;
        }
        UpdateTimestamp();
    }
}
