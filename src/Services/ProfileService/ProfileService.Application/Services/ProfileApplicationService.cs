using BuildingBlocks.Common.Results;
using ProfileService.Application.DTOs;
using ProfileService.Application.Interfaces;
using ProfileService.Domain.Entities;
using ProfileService.Domain.Enums;
using ProfileService.Domain.Repositories;

namespace ProfileService.Application.Services;

public class ProfileApplicationService(
    ICandidateProfileRepository candidateRepo,
    IRecruiterProfileRepository recruiterRepo,
    IAddressRepository addressRepo,
    IResumeRepository resumeRepo,
    IProfileUnitOfWork unitOfWork,
    IStorageService storageService)
{
    // ── Candidate ──────────────────────────────────────────────────────────
    public async Task<Result<CandidateProfileResponse>> GetCandidateProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var profile = await candidateRepo.GetByUserIdAsync(userId, ct);
        if (profile is null)
        {
            profile = CandidateProfile.Create(userId);
            await candidateRepo.AddAsync(profile, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }
        return Result.Success(MapCandidate(profile));
    }

    public async Task<Result<CandidateProfileResponse>> UpdateCandidateProfileAsync(
        Guid userId, UpdateCandidateProfileRequest request, CancellationToken ct = default)
    {
        var profile = await candidateRepo.GetByUserIdAsync(userId, ct);
        if (profile is null)
        {
            profile = CandidateProfile.Create(userId);
            await candidateRepo.AddAsync(profile, ct);
        }

        if (!Enum.TryParse<ExperienceLevel>(request.ExperienceLevel, true, out var level))
            return Result.Failure<CandidateProfileResponse>(Error.Custom("Profile.InvalidLevel", "Invalid experience level."));

        profile.Update(request.Headline, request.Summary, request.Phone, request.LinkedInUrl,
            request.GitHubUrl, request.PortfolioUrl, level, request.YearsOfExperience,
            request.ExpectedSalary, request.Currency, request.IsOpenToWork);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(MapCandidate(profile));
    }

    // ── Recruiter ──────────────────────────────────────────────────────────
    public async Task<Result<RecruiterProfileResponse>> GetRecruiterProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var profile = await recruiterRepo.GetByUserIdAsync(userId, ct);
        if (profile is null)
        {
            profile = RecruiterProfile.Create(userId);
            await recruiterRepo.AddAsync(profile, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }
        return Result.Success(MapRecruiter(profile));
    }

    public async Task<Result<RecruiterProfileResponse>> UpdateRecruiterProfileAsync(
        Guid userId, UpdateRecruiterProfileRequest request, CancellationToken ct = default)
    {
        var profile = await recruiterRepo.GetByUserIdAsync(userId, ct);
        if (profile is null)
        {
            profile = RecruiterProfile.Create(userId);
            await recruiterRepo.AddAsync(profile, ct);
        }

        profile.Update(request.CompanyName, request.CompanyWebsite, request.CompanyLogoUrl,
            request.Industry, request.CompanySize, request.CompanyDescription,
            request.ContactEmail, request.ContactPhone, request.LinkedInUrl);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(MapRecruiter(profile));
    }

    // ── Resume ─────────────────────────────────────────────────────────────
    public async Task<Result<ResumeDocumentDto>> UploadResumeAsync(
        Guid userId, UploadResumeRequest request, CancellationToken ct = default)
    {
        var profile = await candidateRepo.GetByUserIdAsync(userId, ct);
        if (profile is null)
            return Result.Failure<ResumeDocumentDto>(Error.Custom("Profile.NotFound", "Candidate profile not found."));

        var storageKey = $"resumes/{userId}/{Guid.NewGuid()}/{request.FileName}";
        await storageService.UploadAsync(storageKey, request.FileStream, request.ContentType, ct);
        var publicUrl = await storageService.GetPresignedUrlAsync(storageKey, TimeSpan.FromDays(7), ct);

        var resume = ResumeDocument.Create(profile.Id, request.FileName, storageKey,
            request.ContentType, request.FileSizeBytes, request.IsPrimary);
        resume.SetPublicUrl(publicUrl);

        profile.AddResume(resume);
        await resumeRepo.AddAsync(resume, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new ResumeDocumentDto(resume.Id, resume.FileName, resume.ContentType,
            resume.FileSizeBytes, resume.IsPrimary, resume.PublicUrl, resume.CreatedAt));
    }

    public async Task<Result> DeleteResumeAsync(Guid userId, Guid resumeId, CancellationToken ct = default)
    {
        var resume = await resumeRepo.GetByIdAsync(resumeId, ct);
        if (resume is null || resume.CandidateProfileId == Guid.Empty)
            return Result.Failure(Error.NotFound);

        await storageService.DeleteAsync(resume.StorageKey, ct);
        await resumeRepo.DeleteAsync(resumeId, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    // ── Address ────────────────────────────────────────────────────────────
    public async Task<Result<AddressDto>> AddCandidateAddressAsync(
        Guid userId, AddAddressRequest request, CancellationToken ct = default)
    {
        var profile = await candidateRepo.GetByUserIdAsync(userId, ct);
        if (profile is null)
            return Result.Failure<AddressDto>(Error.Custom("Profile.NotFound", "Profile not found."));

        var address = Address.CreateForCandidate(profile.Id, request.Street, request.City,
            request.State, request.Country, request.ZipCode, request.IsPrimary);
        profile.AddAddress(address);
        await addressRepo.AddAsync(address, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AddressDto(address.Id, address.Street, address.City,
            address.State, address.Country, address.ZipCode, address.IsPrimary));
    }

    public async Task<Result<AddressDto>> UpdateAddressAsync(
        Guid addressId, UpdateAddressRequest request, CancellationToken ct = default)
    {
        var address = await addressRepo.GetByIdAsync(addressId, ct);
        if (address is null) return Result.Failure<AddressDto>(Error.NotFound);

        address.Update(request.Street, request.City, request.State,
            request.Country, request.ZipCode, request.IsPrimary);
        await addressRepo.UpdateAsync(address, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AddressDto(address.Id, address.Street, address.City,
            address.State, address.Country, address.ZipCode, address.IsPrimary));
    }

    // ── Mapping Helpers ────────────────────────────────────────────────────
    private static CandidateProfileResponse MapCandidate(CandidateProfile p) => new(
        p.Id, p.UserId, p.Headline, p.Summary, p.Phone, p.LinkedInUrl, p.GitHubUrl,
        p.PortfolioUrl, p.ExperienceLevel.ToString(), p.YearsOfExperience, p.ExpectedSalary,
        p.Currency, p.IsOpenToWork, p.ProfileCompletionPercent,
        p.Skills.Select(s => new SkillDto(s.Id, s.Name, s.ProficiencyLevel, s.YearsOfExperience)).ToList(),
        p.Experiences.Select(e => new WorkExperienceDto(e.Id, e.Title, e.Company, e.Location,
            e.StartDate, e.EndDate, e.IsCurrentRole, e.Description)).ToList(),
        p.Addresses.Select(a => new AddressDto(a.Id, a.Street, a.City, a.State, a.Country, a.ZipCode, a.IsPrimary)).ToList(),
        p.Resumes.Select(r => new ResumeDocumentDto(r.Id, r.FileName, r.ContentType, r.FileSizeBytes, r.IsPrimary, r.PublicUrl, r.CreatedAt)).ToList()
    );

    private static RecruiterProfileResponse MapRecruiter(RecruiterProfile p) => new(
        p.Id, p.UserId, p.CompanyName, p.CompanyWebsite, p.CompanyLogoUrl,
        p.Industry, p.CompanySize, p.CompanyDescription, p.ContactEmail,
        p.ContactPhone, p.LinkedInUrl, p.ProfileCompletionPercent,
        p.Addresses.Select(a => new AddressDto(a.Id, a.Street, a.City, a.State, a.Country, a.ZipCode, a.IsPrimary)).ToList()
    );
}
