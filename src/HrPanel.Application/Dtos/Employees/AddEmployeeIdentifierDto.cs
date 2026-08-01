using HrPanel.Domain.Employees;

namespace HrPanel.Application.Dtos.Employees;

public sealed class AddEmployeeIdentifierDto
{
    public IdentifierType Type { get; set; }
    public string Value { get; set; } = string.Empty;
    public DateOnly? EffectiveFrom { get; set; }
}
