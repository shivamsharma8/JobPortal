using BuildingBlocks.Common.Domain;
using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Entities;

public sealed class Notification : BaseEntity
{
    private Notification() { }

    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public NotificationType Type { get; private set; }
    public Guid? ReferenceId { get; private set; }
    public bool IsRead { get; private set; }

    public static Notification Create(
        Guid userId,
        string title,
        string message,
        NotificationType type,
        Guid? referenceId = null)
    {
        return new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            ReferenceId = referenceId,
            IsRead = false
        };
    }

    public void MarkAsRead()
    {
        IsRead = true;
        UpdateTimestamp();
    }
}
