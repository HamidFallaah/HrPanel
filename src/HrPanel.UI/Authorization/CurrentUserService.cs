using System.Security.Claims;
using HrPanel.Application.Common.Abstractions.Services;
using HrPanel.Application.Common.Authorization;
using Microsoft.AspNetCore.Http;

namespace HrPanel.UI.Authorization;

internal sealed class CurrentUserService: ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirstValue( ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId)? userId: null;
        }
    }

    public long? EmployeeId
    {
        get
        {
            var value = User?.FindFirstValue(CustomClaimTypes.EmployeeId);

            return long.TryParse(value, out var employeeId)? employeeId: null;
        }
    }

    public bool IsAuthenticated =>User?.Identity?.IsAuthenticated == true;

    public bool IsInRole(string role)
    {
        return User?.IsInRole(role) == true;
    }
}