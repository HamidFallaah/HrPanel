using HrPanel.Domain.Employees;

namespace HrPanel.Application.Dtos.Employees;

public sealed class AddEmployeeContactDto
{
    public ContactType Type { get; set; }
    public string Value { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}
