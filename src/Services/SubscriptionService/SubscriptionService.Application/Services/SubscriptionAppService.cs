using BuildingBlocks.Common.Results;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Domain.Entities;
using SubscriptionService.Domain.Enums;
using SubscriptionService.Domain.Repositories;

namespace SubscriptionService.Application.Services;

public class SubscriptionAppService(
    ISubscriptionRepository subscriptionRepository,
    IPaymentRepository paymentRepository,
    ISubscriptionUnitOfWork unitOfWork)
{
    public async Task<Result<SubscriptionResponse>> GetMySubscriptionAsync(
        Guid recruiterId, CancellationToken ct = default)
    {
        var subscription = await subscriptionRepository.GetByRecruiterAsync(recruiterId, ct);
        if (subscription is null)
            return Result.Failure<SubscriptionResponse>(Error.NotFound);

        subscription.CheckAndExpire();
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(Map(subscription));
    }

    public async Task<Result<SubscriptionResponse>> SubscribeAsync(
        Guid recruiterId, CreateSubscriptionRequest request, CancellationToken ct = default)
    {
        if (!Enum.TryParse<SubscriptionPlan>(request.Plan, true, out var plan))
            return Result.Failure<SubscriptionResponse>(
                Error.Custom("Subscription.InvalidPlan", "Invalid subscription plan."));

        var existing = await subscriptionRepository.GetByRecruiterAsync(recruiterId, ct);

        if (existing is not null)
        {
            existing.Upgrade(plan, request.DurationDays);
            await subscriptionRepository.UpdateAsync(existing, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success(Map(existing));
        }

        var subscription = Subscription.Create(recruiterId, plan, request.DurationDays);
        await subscriptionRepository.AddAsync(subscription, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(Map(subscription));
    }

    public async Task<Result<PaymentResponse>> RecordPaymentAsync(
        Guid recruiterId, RecordPaymentRequest request, CancellationToken ct = default)
    {
        var subscription = await subscriptionRepository.GetByIdAsync(request.SubscriptionId, ct);
        if (subscription is null)
            return Result.Failure<PaymentResponse>(Error.NotFound);

        if (subscription.RecruiterId != recruiterId)
            return Result.Failure<PaymentResponse>(Error.Forbidden);

        var payment = Payment.Create(
            request.SubscriptionId,
            recruiterId,
            request.Amount,
            request.Currency,
            request.PaymentMethod,
            request.TransactionId);

        await paymentRepository.AddAsync(payment, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(MapPayment(payment));
    }

    public async Task<Result<IReadOnlyList<PaymentResponse>>> GetBillingHistoryAsync(
        Guid recruiterId, CancellationToken ct = default)
    {
        var payments = await paymentRepository.GetByRecruiterAsync(recruiterId, ct);
        return Result.Success(payments.Select(MapPayment).ToList() as IReadOnlyList<PaymentResponse>);
    }

    public async Task<Result<IReadOnlyList<SubscriptionResponse>>> GetAllSubscriptionsAsync(
        CancellationToken ct = default)
    {
        var subscriptions = await subscriptionRepository.GetAllAsync(ct);
        return Result.Success(subscriptions.Select(Map).ToList() as IReadOnlyList<SubscriptionResponse>);
    }

    private static SubscriptionResponse Map(Subscription s) => new(
        s.Id, s.RecruiterId, s.Plan.ToString(), s.Status.ToString(),
        s.StartsAt, s.ExpiresAt, s.IsActive(), s.CreatedAt, s.UpdatedAt);

    private static PaymentResponse MapPayment(Payment p) => new(
        p.Id, p.SubscriptionId, p.RecruiterId,
        p.Amount, p.Currency, p.PaymentMethod,
        p.TransactionId, p.Status.ToString(), p.PaidAt);
}
