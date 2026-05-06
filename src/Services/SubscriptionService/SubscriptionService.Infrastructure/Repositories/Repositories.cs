using Microsoft.EntityFrameworkCore;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Repositories;
using SubscriptionService.Infrastructure.Data;

namespace SubscriptionService.Infrastructure.Repositories;

public class SubscriptionRepository(SubscriptionDbContext context) : ISubscriptionRepository
{
    public Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => context.Subscriptions.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Subscription?> GetByRecruiterAsync(Guid recruiterId, CancellationToken ct = default)
        => context.Subscriptions.FirstOrDefaultAsync(x => x.RecruiterId == recruiterId, ct);

    public async Task<IReadOnlyList<Subscription>> GetAllAsync(CancellationToken ct = default)
        => await context.Subscriptions.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);

    public async Task AddAsync(Subscription subscription, CancellationToken ct = default)
        => await context.Subscriptions.AddAsync(subscription, ct);

    public Task UpdateAsync(Subscription subscription, CancellationToken ct = default)
    {
        context.Subscriptions.Update(subscription);
        return Task.CompletedTask;
    }
}

public class PaymentRepository(SubscriptionDbContext context) : IPaymentRepository
{
    public async Task<IReadOnlyList<Payment>> GetByRecruiterAsync(Guid recruiterId, CancellationToken ct = default)
        => await context.Payments
            .Where(x => x.RecruiterId == recruiterId)
            .OrderByDescending(x => x.PaidAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Payment>> GetBySubscriptionAsync(Guid subscriptionId, CancellationToken ct = default)
        => await context.Payments
            .Where(x => x.SubscriptionId == subscriptionId)
            .OrderByDescending(x => x.PaidAt)
            .ToListAsync(ct);

    public async Task AddAsync(Payment payment, CancellationToken ct = default)
        => await context.Payments.AddAsync(payment, ct);
}

public class SubscriptionUnitOfWork(SubscriptionDbContext context) : ISubscriptionUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => context.SaveChangesAsync(ct);
}
