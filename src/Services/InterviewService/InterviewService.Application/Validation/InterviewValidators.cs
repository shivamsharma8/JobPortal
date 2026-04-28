using FluentValidation;
using InterviewService.Application.DTOs;
using InterviewService.Domain.Enums;

namespace InterviewService.Application.Validation;

public class ScheduleInterviewRequestValidator : AbstractValidator<ScheduleInterviewRequest>
{
    public ScheduleInterviewRequestValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.ScheduledAt).Must(date => date > DateTime.UtcNow).WithMessage("ScheduledAt must be in the future.");
        RuleFor(x => x.Mode).NotEmpty().Must(mode => Enum.TryParse<InterviewMode>(mode, true, out _)).WithMessage("Mode must be Online or InPerson.");
        RuleFor(x => x.MeetingLink).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.MeetingLink));
        RuleFor(x => x.Location).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Location));
        RuleFor(x => x.Notes).MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}

public class RequestRescheduleInterviewRequestValidator : AbstractValidator<RequestRescheduleInterviewRequest>
{
    public RequestRescheduleInterviewRequestValidator()
    {
        RuleFor(x => x.Notes).MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
