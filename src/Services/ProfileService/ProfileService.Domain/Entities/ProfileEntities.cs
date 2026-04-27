using BuildingBlocks.Common.Domain;

namespace ProfileService.Domain.Entities;

public sealed class RecruiterProfile : BaseEntity
{
    private RecruiterProfile() { }

    public Guid UserId { get; private set; }
    public string CompanyName { get; private set; } = string.Empty;
    public string CompanyWebsite { get; private set; } = string.Empty;
    public string CompanyLogoUrl { get; private set; } = string.Empty;
    public string Industry { get; private set; } = string.Empty;
    public string CompanySize { get; private set; } = string.Empty;
    public string CompanyDescription { get; private set; } = string.Empty;
    public string ContactEmail { get; private set; } = string.Empty;
    public string ContactPhone { get; private set; } = string.Empty;
    public string LinkedInUrl { get; private set; } = string.Empty;
    public int ProfileCompletionPercent { get; private set; }

    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();
    private readonly List<Address> _addresses = [];

    public static RecruiterProfile Create(Guid userId) => new() { UserId = userId };

    public void Update(string companyName, string website, string logoUrl, string industry,
        string size, string description, string contactEmail, string contactPhone, string linkedIn)
    {
        CompanyName = companyName;
        CompanyWebsite = website;
        CompanyLogoUrl = logoUrl;
        Industry = industry;
        CompanySize = size;
        CompanyDescription = description;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
        LinkedInUrl = linkedIn;
        RecalculateCompletion();
        UpdateTimestamp();
    }

    public void AddAddress(Address address) { _addresses.Add(address); RecalculateCompletion(); UpdateTimestamp(); }

    private void RecalculateCompletion()
    {
        int score = 0;
        if (!string.IsNullOrWhiteSpace(CompanyName)) score += 25;
        if (!string.IsNullOrWhiteSpace(CompanyDescription)) score += 25;
        if (!string.IsNullOrWhiteSpace(Industry)) score += 20;
        if (!string.IsNullOrWhiteSpace(ContactEmail)) score += 15;
        if (_addresses.Count > 0) score += 15;
        ProfileCompletionPercent = Math.Min(score, 100);
    }
}

public sealed class Skill : BaseEntity
{
    private Skill() { }
    public Guid CandidateProfileId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int ProficiencyLevel { get; private set; } // 1-5
    public int YearsOfExperience { get; private set; }

    public static Skill Create(Guid profileId, string name, int proficiency, int years)
        => new() { CandidateProfileId = profileId, Name = name, ProficiencyLevel = proficiency, YearsOfExperience = years };
}

public sealed class WorkExperience : BaseEntity
{
    private WorkExperience() { }
    public Guid CandidateProfileId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Company { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsCurrentRole { get; private set; }
    public string Description { get; private set; } = string.Empty;

    public static WorkExperience Create(Guid profileId, string title, string company, string location,
        DateTime startDate, DateTime? endDate, bool isCurrent, string description)
        => new()
        {
            CandidateProfileId = profileId, Title = title, Company = company,
            Location = location, StartDate = startDate, EndDate = endDate,
            IsCurrentRole = isCurrent, Description = description
        };
}

public sealed class Address : BaseEntity
{
    private Address() { }
    public Guid? CandidateProfileId { get; private set; }
    public Guid? RecruiterProfileId { get; private set; }
    public string Street { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string ZipCode { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }

    public static Address CreateForCandidate(Guid profileId, string street, string city,
        string state, string country, string zip, bool isPrimary)
        => new()
        {
            CandidateProfileId = profileId, Street = street, City = city,
            State = state, Country = country, ZipCode = zip, IsPrimary = isPrimary
        };

    public static Address CreateForRecruiter(Guid profileId, string street, string city,
        string state, string country, string zip, bool isPrimary)
        => new()
        {
            RecruiterProfileId = profileId, Street = street, City = city,
            State = state, Country = country, ZipCode = zip, IsPrimary = isPrimary
        };

    public void Update(string street, string city, string state, string country, string zip, bool isPrimary)
    {
        Street = street; City = city; State = state;
        Country = country; ZipCode = zip; IsPrimary = isPrimary;
        UpdateTimestamp();
    }
}

public sealed class ResumeDocument : BaseEntity
{
    private ResumeDocument() { }
    public Guid CandidateProfileId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string StorageKey { get; private set; } = string.Empty;   // S3 object key
    public string ContentType { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }
    public bool IsPrimary { get; private set; }
    public string? PublicUrl { get; private set; }

    public static ResumeDocument Create(Guid profileId, string fileName, string storageKey,
        string contentType, long fileSize, bool isPrimary)
        => new()
        {
            CandidateProfileId = profileId, FileName = fileName, StorageKey = storageKey,
            ContentType = contentType, FileSizeBytes = fileSize, IsPrimary = isPrimary
        };

    public void SetPublicUrl(string url) { PublicUrl = url; UpdateTimestamp(); }
}
