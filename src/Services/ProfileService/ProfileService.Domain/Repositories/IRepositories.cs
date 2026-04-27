using ProfileService.Domain.Entities;

namespace ProfileService.Domain.Repositories;

public interface ICandidateProfileRepository
{
    Task<CandidateProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<CandidateProfile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(CandidateProfile profile, CancellationToken ct = default);
    Task UpdateAsync(CandidateProfile profile, CancellationToken ct = default);
}

public interface IRecruiterProfileRepository
{
    Task<RecruiterProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<RecruiterProfile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(RecruiterProfile profile, CancellationToken ct = default);
    Task UpdateAsync(RecruiterProfile profile, CancellationToken ct = default);
}

public interface IAddressRepository
{
    Task<Address?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Address address, CancellationToken ct = default);
    Task UpdateAsync(Address address, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IResumeRepository
{
    Task<ResumeDocument?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(ResumeDocument resume, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IProfileUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
