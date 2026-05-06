namespace SubscriptionService.Application.DTOs;

public sealed record CreateSubscriptionRequest(
    string Plan,
    int DurationDays = 30);

public sealed record RecordPaymentRequest(
    Guid SubscriptionId,
    decimal Amount,
    string Currency,
    string PaymentMethod,
    string TransactionId);

public sealed record SubscriptionResponse(
    Guid Id,
    Guid RecruiterId,
    string Plan,
    string Status,
    DateTime StartsAt,
    DateTime ExpiresAt,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record PaymentResponse(
    Guid Id,
    Guid SubscriptionId,
    Guid RecruiterId,
    decimal Amount,
    string Currency,
    string PaymentMethod,
    string TransactionId,
    string Status,
    DateTime PaidAt);
