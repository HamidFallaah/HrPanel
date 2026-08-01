using HrPanel.Domain.Employees;

namespace HrPanel.Application.Dtos.Employees;

public sealed class AddEmployeeDependentDto
{
    public string FullName { get; set; } = string.Empty;
    public string? NationalCode { get; set; }
    public DateOnly? BirthDate { get; set; }
    public DependentRelationshipType RelationshipType { get; set; }
    public bool IsEmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
}
