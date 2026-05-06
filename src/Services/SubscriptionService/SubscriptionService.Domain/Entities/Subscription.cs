using BuildingBlocks.Common.Domain;
using SubscriptionService.Domain.Enums;

namespace SubscriptionService.Domain.Entities;

public sealed class Subscription : BaseEntity
{
    private Subscription() { }

    public Guid RecruiterId { get; private set; }
    public SubscriptionPlan Plan { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime StartsAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    public static Subscription Create(Guid recruiterId, SubscriptionPlan plan, int durationDays)
    {
        var now = DateTime.UtcNow;
        return new Subscription
        {
            RecruiterId = recruiterId,
            Plan = plan,
            Status = SubscriptionStatus.Active,
            StartsAt = now,
            ExpiresAt = now.AddDays(durationDays)
        };
    }

    public void Upgrade(SubscriptionPlan newPlan, int durationDays)
    {
        Plan = newPlan;
        Status = SubscriptionStatus.Active;
        StartsAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddDays(durationDays);
        UpdateTimestamp();
    }

    public void Cancel()
    {
        Status = SubscriptionStatus.Cancelled;
        UpdateTimestamp();
    }

    public void CheckAndExpire()
    {
        if (Status == SubscriptionStatus.Active && ExpiresAt < DateTime.UtcNow)
        {
            Status = SubscriptionStatus.Expired;
            UpdateTimestamp();
        }
    }

    public bool IsActive() => Status == SubscriptionStatus.Active && ExpiresAt >= DateTime.UtcNow;
}
