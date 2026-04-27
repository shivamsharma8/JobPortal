using BuildingBlocks.Common.Domain;
using ProfileService.Domain.Enums;

namespace ProfileService.Domain.Entities;

public sealed class CandidateProfile : BaseEntity
{
    private CandidateProfile() { }

    public Guid UserId { get; private set; }
    public string Headline { get; private set; } = string.Empty;
    public string Summary { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string LinkedInUrl { get; private set; } = string.Empty;
    public string GitHubUrl { get; private set; } = string.Empty;
    public string PortfolioUrl { get; private set; } = string.Empty;
    public ExperienceLevel ExperienceLevel { get; private set; } = ExperienceLevel.Fresher;
    public int YearsOfExperience { get; private set; }
    public decimal? ExpectedSalary { get; private set; }
    public string? Currency { get; private set; }
    public bool IsOpenToWork { get; private set; } = true;
    public int ProfileCompletionPercent { get; private set; }

    public IReadOnlyCollection<Skill> Skills => _skills.AsReadOnly();
    private readonly List<Skill> _skills = [];

    public IReadOnlyCollection<WorkExperience> Experiences => _experiences.AsReadOnly();
    private readonly List<WorkExperience> _experiences = [];

    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();
    private readonly List<Address> _addresses = [];

    public IReadOnlyCollection<ResumeDocument> Resumes => _resumes.AsReadOnly();
    private readonly List<ResumeDocument> _resumes = [];

    public static CandidateProfile Create(Guid userId)
        => new() { UserId = userId };

    public void Update(string headline, string summary, string phone, string linkedIn,
        string gitHub, string portfolio, ExperienceLevel level, int years,
        decimal? expectedSalary, string? currency, bool isOpenToWork)
    {
        Headline = headline;
        Summary = summary;
        Phone = phone;
        LinkedInUrl = linkedIn;
        GitHubUrl = gitHub;
        PortfolioUrl = portfolio;
        ExperienceLevel = level;
        YearsOfExperience = years;
        ExpectedSalary = expectedSalary;
        Currency = currency;
        IsOpenToWork = isOpenToWork;
        RecalculateCompletion();
        UpdateTimestamp();
    }

    public void AddSkill(Skill skill) { _skills.Add(skill); RecalculateCompletion(); UpdateTimestamp(); }
    public void RemoveSkill(Guid skillId) { _skills.RemoveAll(s => s.Id == skillId); RecalculateCompletion(); UpdateTimestamp(); }
    public void AddExperience(WorkExperience exp) { _experiences.Add(exp); RecalculateCompletion(); UpdateTimestamp(); }
    public void AddAddress(Address address) { _addresses.Add(address); RecalculateCompletion(); UpdateTimestamp(); }
    public void UpdateAddress(Address address) { UpdateTimestamp(); }
    public void AddResume(ResumeDocument resume) { _resumes.Add(resume); RecalculateCompletion(); UpdateTimestamp(); }
    public void RemoveResume(Guid resumeId) { _resumes.RemoveAll(r => r.Id == resumeId); RecalculateCompletion(); UpdateTimestamp(); }

    private void RecalculateCompletion()
    {
        int score = 0;
        if (!string.IsNullOrWhiteSpace(Headline)) score += 15;
        if (!string.IsNullOrWhiteSpace(Summary)) score += 15;
        if (!string.IsNullOrWhiteSpace(Phone)) score += 10;
        if (_skills.Count > 0) score += 20;
        if (_experiences.Count > 0) score += 20;
        if (_addresses.Count > 0) score += 10;
        if (_resumes.Count > 0) score += 10;
        ProfileCompletionPercent = Math.Min(score, 100);
    }
}
