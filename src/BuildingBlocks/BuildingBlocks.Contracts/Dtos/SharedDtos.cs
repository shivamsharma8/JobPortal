namespace BuildingBlocks.Contracts.Dtos;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    bool IsActive,
    DateTime CreatedAt
);

public record JobSummaryDto(
    Guid Id,
    string Title,
    string Company,
    string Location,
    string JobType,
    string SalaryRange,
    DateTime PostedAt
);
