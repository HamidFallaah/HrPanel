using HrPanel.Domain.Employees;
using Microsoft.AspNetCore.Identity;

namespace HrPanel.Persistence.Identity;

// ApplicationUser represents a login account, while Domain Employee represents an employee record

// An employee can exist without a login account, and an administrator account can exist without an employee record
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public long? EmployeeId { get; private set; }
    public string? DisplayName { get; private set; }
    public Employee? Employee { get; private set; }
    public void LinkToEmployee(long employeeId)
    {
        if (employeeId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(employeeId), "شناسه کارمند باید بزرگتر از صفر باشد");
        }

        EmployeeId = employeeId;
    }

    public void UnlinkEmployee()
    {
        EmployeeId = null;
    }

    public void SetDisplayName(string? displayName)
    {
        DisplayName = string.IsNullOrWhiteSpace(displayName)? null: displayName.Trim();
    }
}