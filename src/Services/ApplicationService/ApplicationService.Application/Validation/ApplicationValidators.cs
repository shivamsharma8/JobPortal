using ApplicationService.Application.DTOs;
using ApplicationService.Domain.Enums;
using FluentValidation;

namespace ApplicationService.Application.Validation;

public class SubmitApplicationRequestValidator : AbstractValidator<SubmitApplicationRequest>
{
    public SubmitApplicationRequestValidator()
    {
        RuleFor(x => x.JobId).NotEmpty();
        RuleFor(x => x.ResumeUrl).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.ResumeUrl));
        RuleFor(x => x.CoverLetter).MaximumLength(4000).When(x => !string.IsNullOrWhiteSpace(x.CoverLetter));
    }
}

public class UpdateApplicationStatusRequestValidator : AbstractValidator<UpdateApplicationStatusRequest>
{
    public UpdateApplicationStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(status => Enum.TryParse<ApplicationStatus>(status, true, out var parsed)
                && parsed is ApplicationStatus.Shortlisted or ApplicationStatus.InterviewScheduled or ApplicationStatus.Offered or ApplicationStatus.Rejected)
            .WithMessage("Status must be Shortlisted, InterviewScheduled, Offered, or Rejected.");
    }
}
