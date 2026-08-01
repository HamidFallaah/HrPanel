namespace HrPanel.Application.Dtos.Employees;

public sealed class AddEmployeeEducationDto
{
    public string? DegreeTitle { get; set; }
    public string? FieldOfStudy { get; set; }
    public string? InstitutionName { get; set; }
    public DateOnly? GraduationDate { get; set; }
    public bool IsHighestDegree { get; set; }
}
