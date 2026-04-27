using Microsoft.EntityFrameworkCore;
using ProfileService.Domain.Entities;
using ProfileService.Domain.Repositories;
using ProfileService.Infrastructure.Data;

namespace ProfileService.Infrastructure.Repositories;

public class CandidateProfileRepository(ProfileDbContext context) : ICandidateProfileRepository
{
    public async Task<CandidateProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await context.CandidateProfiles
            .Include(p => p.Skills)
            .Include(p => p.Experiences)
            .Include(p => p.Addresses)
            .Include(p => p.Resumes)
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

    public async Task<CandidateProfile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.CandidateProfiles
            .Include(p => p.Skills).Include(p => p.Experiences)
            .Include(p => p.Addresses).Include(p => p.Resumes)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AddAsync(CandidateProfile profile, CancellationToken ct = default)
        => await context.CandidateProfiles.AddAsync(profile, ct);

    public Task UpdateAsync(CandidateProfile profile, CancellationToken ct = default)
    {
        context.CandidateProfiles.Update(profile);
        return Task.CompletedTask;
    }
}

public class RecruiterProfileRepository(ProfileDbContext context) : IRecruiterProfileRepository
{
    public async Task<RecruiterProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await context.RecruiterProfiles
            .Include(p => p.Addresses)
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

    public async Task<RecruiterProfile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.RecruiterProfiles
            .Include(p => p.Addresses)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AddAsync(RecruiterProfile profile, CancellationToken ct = default)
        => await context.RecruiterProfiles.AddAsync(profile, ct);

    public Task UpdateAsync(RecruiterProfile profile, CancellationToken ct = default)
    {
        context.RecruiterProfiles.Update(profile);
        return Task.CompletedTask;
    }
}

public class AddressRepository(ProfileDbContext context) : IAddressRepository
{
    public async Task<Address?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Addresses.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task AddAsync(Address address, CancellationToken ct = default)
        => await context.Addresses.AddAsync(address, ct);

    public Task UpdateAsync(Address address, CancellationToken ct = default)
    {
        context.Addresses.Update(address);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var address = await context.Addresses.FindAsync([id], ct);
        if (address is not null) context.Addresses.Remove(address);
    }
}

public class ResumeRepository(ProfileDbContext context) : IResumeRepository
{
    public async Task<ResumeDocument?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.ResumeDocuments.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task AddAsync(ResumeDocument resume, CancellationToken ct = default)
        => await context.ResumeDocuments.AddAsync(resume, ct);

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var resume = await context.ResumeDocuments.FindAsync([id], ct);
        if (resume is not null) context.ResumeDocuments.Remove(resume);
    }
}

public class ProfileUnitOfWork(ProfileDbContext context) : IProfileUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}
