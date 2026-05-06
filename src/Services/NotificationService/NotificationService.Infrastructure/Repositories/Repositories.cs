using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Repositories;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Infrastructure.Repositories;

public class NotificationRepository(NotificationDbContext context) : INotificationRepository
{
    public Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => context.Notifications.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Notification>> GetByUserAsync(Guid userId, CancellationToken ct = default)
        => await context.Notifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
        => context.Notifications.CountAsync(x => x.UserId == userId && !x.IsRead, ct);

    public async Task AddAsync(Notification notification, CancellationToken ct = default)
        => await context.Notifications.AddAsync(notification, ct);

    public Task UpdateAsync(Notification notification, CancellationToken ct = default)
    {
        context.Notifications.Update(notification);
        return Task.CompletedTask;
    }

    public async Task UpdateAllAsReadAsync(Guid userId, CancellationToken ct = default)
    {
        await context.Notifications
            .Where(x => x.UserId == userId && !x.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), ct);
    }
}

public class NotificationUnitOfWork(NotificationDbContext context) : INotificationUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => context.SaveChangesAsync(ct);
}
