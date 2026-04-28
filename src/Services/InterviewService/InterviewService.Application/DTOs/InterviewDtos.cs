namespace InterviewService.Application.DTOs;

public sealed record ScheduleInterviewRequest(
    Guid ApplicationId,
    DateTime ScheduledAt,
    string Mode,
    string? MeetingLink,
    string? Location,
    string? Notes);

public sealed record RequestRescheduleInterviewRequest(string? Notes);

public sealed record InterviewResponse(
    Guid Id,
    Guid ApplicationId,
    Guid CandidateId,
    Guid RecruiterId,
    DateTime ScheduledAt,
    string Mode,
    string MeetingLink,
    string Location,
    string Notes,
    string Status,
    DateTime UpdatedAt);

public sealed record ApplicationLookupResponse(
    Guid Id,
    Guid JobId,
    Guid CandidateId,
    Guid RecruiterId,
    string ResumeUrl,
    string CoverLetter,
    string Status,
    DateTime AppliedAt,
    DateTime UpdatedAt);
