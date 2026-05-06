namespace AnalyticsService.Application.DTOs;

public sealed record JobStatResponse(
    Guid JobId,
    Guid RecruiterId,
    int TotalApplications,
    int Shortlisted,
    int InterviewsScheduled,
    int Offered,
    int Rejected,
    int Withdrawn,
    DateTime? FirstApplicationAt,
    DateTime? LatestApplicationAt,
    DateTime UpdatedAt);

public sealed record RecruiterDashboardResponse(
    Guid RecruiterId,
    int TotalJobs,
    int TotalApplications,
    int TotalShortlisted,
    int TotalInterviews,
    int TotalOffers,
    int TotalRejected);

public sealed record PlatformStatResponse(
    DateOnly Date,
    int TotalJobs,
    int TotalApplications,
    int TotalInterviews,
    int TotalOffers);
