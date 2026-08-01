namespace HrPanel.Application.Dtos.Identity;

public sealed record CurrentUserDto(Guid UserId,string UserName,string? Email,string? DisplayName,long? EmployeeId,IReadOnlyCollection<string> Roles);