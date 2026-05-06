using BuildingBlocks.Common.Domain;

namespace AnalyticsService.Domain.Entities;

/// <summary>
/// Platform-wide aggregated daily snapshot. One row per date.
/// </summary>
public sealed class PlatformStat : BaseEntity
{
    private PlatformStat() { }

    public DateOnly Date { get; private set; }
    public int TotalJobs { get; private set; }
    public int TotalApplications { get; private set; }
    public int TotalInterviews { get; private set; }
    public int TotalOffers { get; private set; }

    public static PlatformStat CreateForToday()
    {
        return new PlatformStat { Date = DateOnly.FromDateTime(DateTime.UtcNow) };
    }

    public void IncrementJobs() { TotalJobs++; UpdateTimestamp(); }
    public void IncrementApplications() { TotalApplications++; UpdateTimestamp(); }
    public void IncrementInterviews() { TotalInterviews++; UpdateTimestamp(); }
    public void IncrementOffers() { TotalOffers++; UpdateTimestamp(); }
}
