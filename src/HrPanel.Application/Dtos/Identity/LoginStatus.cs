namespace HrPanel.Application.Dtos.Identity;

public enum LoginStatus
{
    Succeeded = 1,
    InvalidCredentials = 2,
    LockedOut = 3,
    NotAllowed = 4,
    RequiresTwoFactor = 5
}