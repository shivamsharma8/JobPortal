namespace AuthService.Domain.Enums;

public enum UserRole
{
    Candidate = 1,
    Recruiter = 2,
    Admin = 3
}

public enum AuthProvider
{
    Local = 1,
    GitHub = 2,
    Google = 3
}
