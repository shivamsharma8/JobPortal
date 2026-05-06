namespace NotificationService.Application.DTOs;

public sealed record NotificationResponse(
    Guid Id,
    Guid UserId,
    string Title,
    string Message,
    string Type,
    Guid? ReferenceId,
    bool IsRead,
    DateTime CreatedAt);

public sealed record UnreadCountResponse(int Count);
