namespace JobService.Domain.Enums;

public enum JobStatus
{
    Draft = 1,
    Active = 2,
    Paused = 3,
    Closed = 4,
    Expired = 5
}

public enum JobType
{
    FullTime = 1,
    PartTime = 2,
    Contract = 3,
    Freelance = 4,
    Internship = 5,
    Remote = 6
}

public enum ExperienceLevel
{
    Fresher = 1,
    Junior = 2,
    Mid = 3,
    Senior = 4,
    Lead = 5,
    Executive = 6
}
