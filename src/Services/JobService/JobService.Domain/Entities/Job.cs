using BuildingBlocks.Common.Domain;
using JobService.Domain.Enums;

namespace JobService.Domain.Entities;

public sealed class Job : BaseEntity
{
    private Job() { }

    public Guid RecruiterId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Company { get; private set; } = string.Empty;
    public string CompanyLogoUrl { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public bool IsRemote { get; private set; }
    public JobType JobType { get; private set; }
    public ExperienceLevel ExperienceLevel { get; private set; }
    public string SalaryMin { get; private set; } = string.Empty;
    public string SalaryMax { get; private set; } = string.Empty;
    public string Currency { get; private set; } = "USD";
    public JobStatus Status { get; private set; } = JobStatus.Draft;
    public DateTime? PublishedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public Guid CategoryId { get; private set; }
    public int ViewCount { get; private set; }
    public int ApplicationCount { get; private set; }

    public IReadOnlyCollection<JobSkill> Skills => _skills.AsReadOnly();
    private readonly List<JobSkill> _skills = [];

    public static Job Create(Guid recruiterId, string title, string description, string company,
        string companyLogoUrl, string location, bool isRemote, JobType jobType,
        ExperienceLevel level, string salaryMin, string salaryMax, string currency,
        Guid categoryId, DateTime? expiresAt)
    {
        var job = new Job
        {
            RecruiterId = recruiterId,
            Title = title,
            Description = description,
            Company = company,
            CompanyLogoUrl = companyLogoUrl,
            Location = location,
            IsRemote = isRemote,
            JobType = jobType,
            ExperienceLevel = level,
            SalaryMin = salaryMin,
            SalaryMax = salaryMax,
            Currency = currency,
            CategoryId = categoryId,
            ExpiresAt = expiresAt,
            Status = JobStatus.Active,
            PublishedAt = DateTime.UtcNow
        };
        job.AddDomainEvent(new Events.JobPostedDomainEvent(job.Id, job.RecruiterId, job.Title, job.Company));
        return job;
    }

    public void Update(string title, string description, string location, bool isRemote,
        JobType jobType, ExperienceLevel level, string salaryMin, string salaryMax,
        string currency, Guid categoryId, DateTime? expiresAt)
    {
        Title = title;
        Description = description;
        Location = location;
        IsRemote = isRemote;
        JobType = jobType;
        ExperienceLevel = level;
        SalaryMin = salaryMin;
        SalaryMax = salaryMax;
        Currency = currency;
        CategoryId = categoryId;
        ExpiresAt = expiresAt;
        UpdateTimestamp();
    }

    public void Pause()
    {
        if (Status != JobStatus.Active) throw new InvalidOperationException("Only active jobs can be paused.");
        Status = JobStatus.Paused;
        UpdateTimestamp();
    }

    public void Activate()
    {
        if (Status != JobStatus.Paused) throw new InvalidOperationException("Only paused jobs can be activated.");
        Status = JobStatus.Active;
        UpdateTimestamp();
    }

    public void Close() { Status = JobStatus.Closed; UpdateTimestamp(); }

    public void AddSkill(JobSkill skill) { _skills.Add(skill); UpdateTimestamp(); }
    public void ClearSkills() { _skills.Clear(); }
    public void IncrementViewCount() { ViewCount++; }
    public void IncrementApplicationCount() { ApplicationCount++; }
}
