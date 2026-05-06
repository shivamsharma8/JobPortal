using BuildingBlocks.Common.Domain;
using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Domain.Entities;

public sealed class Payment : BaseEntity
{
    private Payment() { }

    public Guid SubscriptionId { get; private set; }
    public Guid RecruiterId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "INR";
    public string PaymentMethod { get; private set; } = string.Empty;
    public string TransactionId { get; private set; } = string.Empty;
    public PaymentStatus Status { get; private set; }
    public DateTime PaidAt { get; private set; }

    public static Payment Create(
        Guid subscriptionId,
        Guid recruiterId,
        decimal amount,
        string currency,
        string paymentMethod,
        string transactionId)
    {
        return new Payment
        {
            SubscriptionId = subscriptionId,
            RecruiterId = recruiterId,
            Amount = amount,
            Currency = currency,
            PaymentMethod = paymentMethod,
            TransactionId = transactionId,
            Status = PaymentStatus.Completed,
            PaidAt = DateTime.UtcNow
        };
    }
}
