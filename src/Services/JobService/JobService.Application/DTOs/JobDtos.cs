using JobService.Domain.Enums;

namespace JobService.Application.DTOs;

public record CreateJobRequest(
    string Title,
    string Description,
    string Company,
    string CompanyLogoUrl,
    string Location,
    bool IsRemote,
    string JobType,
    string ExperienceLevel,
    string SalaryMin,
    string SalaryMax,
    string Currency,
    Guid CategoryId,
    DateTime? ExpiresAt,
    IReadOnlyList<JobSkillRequest> Skills
);

public record UpdateJobRequest(
    string Title,
    string Description,
    string Location,
    bool IsRemote,
    string JobType,
    string ExperienceLevel,
    string SalaryMin,
    string SalaryMax,
    string Currency,
    Guid CategoryId,
    DateTime? ExpiresAt,
    IReadOnlyList<JobSkillRequest> Skills
);

public record JobSkillRequest(string Name, bool IsRequired);

public record JobResponse(
    Guid Id,
    Guid RecruiterId,
    string Title,
    string Description,
    string Company,
    string CompanyLogoUrl,
    string Location,
    bool IsRemote,
    string JobType,
    string ExperienceLevel,
    string SalaryMin,
    string SalaryMax,
    string Currency,
    string Status,
    DateTime? PublishedAt,
    DateTime? ExpiresAt,
    Guid CategoryId,
    int ViewCount,
    int ApplicationCount,
    IReadOnlyList<JobSkillDto> Skills,
    DateTime CreatedAt
);

public record JobSkillDto(Guid Id, string Name, bool IsRequired);

public record JobSearchRequest(
    string? Q,
    string? Location,
    Guid? CategoryId,
    string? JobType,
    string? ExperienceLevel,
    int PageNumber = 1,
    int PageSize = 20
);
