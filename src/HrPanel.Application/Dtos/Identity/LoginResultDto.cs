namespace HrPanel.Application.Dtos.Identity;

public sealed record LoginResultDto(LoginStatus Status,CurrentUserDto? User = null)
{
    public bool Succeeded => Status == LoginStatus.Succeeded;
}