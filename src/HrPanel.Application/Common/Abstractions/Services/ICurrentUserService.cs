namespace HrPanel.Application.Common.Abstractions.Services;

// The implementation will later read claims from HttpContext
public interface ICurrentUserService
{
    Guid? UserId { get; }
    long? EmployeeId { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}