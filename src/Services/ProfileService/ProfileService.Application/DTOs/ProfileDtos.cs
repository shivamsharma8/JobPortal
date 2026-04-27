using ProfileService.Domain.Enums;

namespace ProfileService.Application.DTOs;

// ── Candidate ───────────────────────────────────────────────────────────────
public record UpdateCandidateProfileRequest(
    string Headline,
    string Summary,
    string Phone,
    string LinkedInUrl,
    string GitHubUrl,
    string PortfolioUrl,
    string ExperienceLevel,
    int YearsOfExperience,
    decimal? ExpectedSalary,
    string? Currency,
    bool IsOpenToWork
);

public record CandidateProfileResponse(
    Guid Id,
    Guid UserId,
    string Headline,
    string Summary,
    string Phone,
    string LinkedInUrl,
    string GitHubUrl,
    string PortfolioUrl,
    string ExperienceLevel,
    int YearsOfExperience,
    decimal? ExpectedSalary,
    string? Currency,
    bool IsOpenToWork,
    int ProfileCompletionPercent,
    IReadOnlyList<SkillDto> Skills,
    IReadOnlyList<WorkExperienceDto> Experiences,
    IReadOnlyList<AddressDto> Addresses,
    IReadOnlyList<ResumeDocumentDto> Resumes
);

public record SkillDto(Guid Id, string Name, int ProficiencyLevel, int YearsOfExperience);
public record AddSkillRequest(string Name, int ProficiencyLevel, int YearsOfExperience);

public record WorkExperienceDto(Guid Id, string Title, string Company, string Location,
    DateTime StartDate, DateTime? EndDate, bool IsCurrentRole, string Description);

// ── Recruiter ───────────────────────────────────────────────────────────────
public record UpdateRecruiterProfileRequest(
    string CompanyName,
    string CompanyWebsite,
    string CompanyLogoUrl,
    string Industry,
    string CompanySize,
    string CompanyDescription,
    string ContactEmail,
    string ContactPhone,
    string LinkedInUrl
);

public record RecruiterProfileResponse(
    Guid Id,
    Guid UserId,
    string CompanyName,
    string CompanyWebsite,
    string CompanyLogoUrl,
    string Industry,
    string CompanySize,
    string CompanyDescription,
    string ContactEmail,
    string ContactPhone,
    string LinkedInUrl,
    int ProfileCompletionPercent,
    IReadOnlyList<AddressDto> Addresses
);

// ── Shared ──────────────────────────────────────────────────────────────────
public record AddressDto(Guid Id, string Street, string City, string State,
    string Country, string ZipCode, bool IsPrimary);

public record AddAddressRequest(string Street, string City, string State,
    string Country, string ZipCode, bool IsPrimary);

public record UpdateAddressRequest(string Street, string City, string State,
    string Country, string ZipCode, bool IsPrimary);

public record ResumeDocumentDto(Guid Id, string FileName, string ContentType,
    long FileSizeBytes, bool IsPrimary, string? PublicUrl, DateTime UploadedAt);

public record UploadResumeRequest(
    string FileName,
    string ContentType,
    long FileSizeBytes,
    bool IsPrimary,
    Stream FileStream
);
