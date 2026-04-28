namespace ApplicationService.Application.DTOs;

public sealed record SubmitApplicationRequest(
    Guid JobId,
    string? ResumeUrl,
    string? CoverLetter,
    bool UseSavedProfile = true);

public sealed record UpdateApplicationStatusRequest(string Status);

public sealed record ApplicationResponse(
    Guid Id,
    Guid JobId,
    Guid CandidateId,
    Guid RecruiterId,
    string ResumeUrl,
    string CoverLetter,
    string Status,
    DateTime AppliedAt,
    DateTime UpdatedAt);

public sealed record JobLookupResponse(
    Guid Id,
    Guid RecruiterId,
    string Title,
    string Company,
    string Status);
