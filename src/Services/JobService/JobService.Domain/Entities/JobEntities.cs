using BuildingBlocks.Common.Domain;

namespace JobService.Domain.Entities;

public sealed class JobSkill : BaseEntity
{
    private JobSkill() { }
    public Guid JobId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public bool IsRequired { get; private set; }

    public static JobSkill Create(Guid jobId, string name, bool isRequired)
        => new() { JobId = jobId, Name = name, IsRequired = isRequired };
}

public sealed class JobCategory : BaseEntity
{
    private JobCategory() { }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int JobCount { get; private set; }

    public static JobCategory Create(string name, string slug, string? description = null)
        => new() { Name = name, Slug = slug, Description = description };
}
