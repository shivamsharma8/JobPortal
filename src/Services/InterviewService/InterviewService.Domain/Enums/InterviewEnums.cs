namespace InterviewService.Domain.Enums;

public enum InterviewMode
{
    Online = 0,
    InPerson = 1
}

public enum InterviewStatus
{
    Scheduled = 0,
    Confirmed = 1,
    RescheduleRequested = 2,
    Cancelled = 3,
    Completed = 4
}
