using BuildingBlocks.Common.Results;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Repositories;

namespace NotificationService.Application.Services;

public class NotificationAppService(
    INotificationRepository notificationRepository,
    INotificationUnitOfWork unitOfWork)
{
    public async Task<Result<IReadOnlyList<NotificationResponse>>> GetMyNotificationsAsync(
        Guid userId, CancellationToken ct = default)
    {
        var notifications = await notificationRepository.GetByUserAsync(userId, ct);
        return Result.Success(notifications.Select(Map).ToList() as IReadOnlyList<NotificationResponse>);
    }

    public async Task<Result<UnreadCountResponse>> GetUnreadCountAsync(
        Guid userId, CancellationToken ct = default)
    {
        var count = await notificationRepository.GetUnreadCountAsync(userId, ct);
        return Result.Success(new UnreadCountResponse(count));
    }

    public async Task<Result<NotificationResponse>> MarkAsReadAsync(
        Guid notificationId, Guid userId, CancellationToken ct = default)
    {
        var notification = await notificationRepository.GetByIdAsync(notificationId, ct);
        if (notification is null) return Result.Failure<NotificationResponse>(Error.NotFound);
        if (notification.UserId != userId) return Result.Failure<NotificationResponse>(Error.Forbidden);

        notification.MarkAsRead();
        await notificationRepository.UpdateAsync(notification, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(Map(notification));
    }

    public async Task<Result<bool>> MarkAllReadAsync(Guid userId, CancellationToken ct = default)
    {
        await notificationRepository.UpdateAllAsReadAsync(userId, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }

    /// <summary>
    /// Called internally by the RabbitMQ consumer to persist a notification.
    /// </summary>
    public async Task CreateNotificationAsync(
        Guid userId,
        string title,
        string message,
        NotificationType type,
        Guid? referenceId = null,
        CancellationToken ct = default)
    {
        var notification = Notification.Create(userId, title, message, type, referenceId);
        await notificationRepository.AddAsync(notification, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private static NotificationResponse Map(Notification n) => new(
        n.Id, n.UserId, n.Title, n.Message,
        n.Type.ToString(), n.ReferenceId, n.IsRead, n.CreatedAt);
}
