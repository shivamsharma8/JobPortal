using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Domain.Repositories;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Subscription?> GetByRecruiterAsync(Guid recruiterId, CancellationToken ct = default);
    Task<IReadOnlyList<Subscription>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Subscription subscription, CancellationToken ct = default);
    Task UpdateAsync(Subscription subscription, CancellationToken ct = default);
}

public interface IPaymentRepository
{
    Task<IReadOnlyList<Payment>> GetByRecruiterAsync(Guid recruiterId, CancellationToken ct = default);
    Task<IReadOnlyList<Payment>> GetBySubscriptionAsync(Guid subscriptionId, CancellationToken ct = default);
    Task AddAsync(Payment payment, CancellationToken ct = default);
}

public interface ISubscriptionUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
