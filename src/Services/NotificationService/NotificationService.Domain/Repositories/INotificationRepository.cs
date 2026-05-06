using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Repositories;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Notification>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Notification notification, CancellationToken ct = default);
    Task UpdateAsync(Notification notification, CancellationToken ct = default);
    Task UpdateAllAsReadAsync(Guid userId, CancellationToken ct = default);
}

public interface INotificationUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
