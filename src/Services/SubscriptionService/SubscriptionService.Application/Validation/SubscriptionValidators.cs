using SubscriptionService.Application.DTOs;
using SubscriptionService.Domain.Enums;
using FluentValidation;

namespace SubscriptionService.Application.Validation;

public class CreateSubscriptionRequestValidator : AbstractValidator<CreateSubscriptionRequest>
{
    public CreateSubscriptionRequestValidator()
    {
        RuleFor(x => x.Plan)
            .NotEmpty()
            .Must(p => Enum.TryParse<SubscriptionPlan>(p, true, out _))
            .WithMessage("Plan must be Free, Pro, or Enterprise.");

        RuleFor(x => x.DurationDays)
            .GreaterThan(0)
            .LessThanOrEqualTo(365)
            .WithMessage("Duration must be between 1 and 365 days.");
    }
}

public class RecordPaymentRequestValidator : AbstractValidator<RecordPaymentRequest>
{
    public RecordPaymentRequestValidator()
    {
        RuleFor(x => x.SubscriptionId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
        RuleFor(x => x.PaymentMethod).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TransactionId).NotEmpty().MaximumLength(200);
    }
}
