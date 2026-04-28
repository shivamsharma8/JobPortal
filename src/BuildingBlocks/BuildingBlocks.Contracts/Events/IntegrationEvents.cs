namespace BuildingBlocks.Contracts.Events;

public record UserRegisteredEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}

public record UserLoggedInEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string IpAddress { get; init; } = string.Empty;
}

public record UserProfileUpdatedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public Guid UserId { get; init; }
    public string Role { get; init; } = string.Empty;
}

public record JobPostedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public Guid JobId { get; init; }
    public Guid RecruiterId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Company { get; init; } = string.Empty;
}

public record ApplicationSubmittedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public Guid ApplicationId { get; init; }
    public Guid JobId { get; init; }
    public Guid CandidateId { get; init; }
    public Guid RecruiterId { get; init; }
    public string ResumeUrl { get; init; } = string.Empty;
    public string CoverLetter { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}

public record ApplicationStatusChangedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public Guid ApplicationId { get; init; }
    public Guid JobId { get; init; }
    public Guid CandidateId { get; init; }
    public Guid RecruiterId { get; init; }
    public string OldStatus { get; init; } = string.Empty;
    public string NewStatus { get; init; } = string.Empty;
}

public record InterviewScheduledEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public Guid InterviewId { get; init; }
    public Guid ApplicationId { get; init; }
    public Guid CandidateId { get; init; }
    public Guid RecruiterId { get; init; }
    public DateTime ScheduledAt { get; init; }
    public string Mode { get; init; } = string.Empty;
}

public record InterviewUpdatedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public Guid InterviewId { get; init; }
    public Guid ApplicationId { get; init; }
    public Guid CandidateId { get; init; }
    public Guid RecruiterId { get; init; }
    public string OldStatus { get; init; } = string.Empty;
    public string NewStatus { get; init; } = string.Empty;
}
