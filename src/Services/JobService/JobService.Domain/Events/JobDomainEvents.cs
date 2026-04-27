using BuildingBlocks.Common.Domain;

namespace JobService.Domain.Events;

public record JobPostedDomainEvent(Guid JobId, Guid RecruiterId, string Title, string Company) : DomainEvent
{
    public override string EventType => "JobPosted";
}

public record JobPausedDomainEvent(Guid JobId, Guid RecruiterId) : DomainEvent
{
    public override string EventType => "JobPaused";
}

public record JobClosedDomainEvent(Guid JobId, Guid RecruiterId) : DomainEvent
{
    public override string EventType => "JobClosed";
}
