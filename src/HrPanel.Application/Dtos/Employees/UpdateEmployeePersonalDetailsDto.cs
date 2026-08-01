using HrPanel.Domain.Employees;

namespace HrPanel.Application.Dtos.Employees;

public sealed class UpdateEmployeePersonalDetailsDto
{
    public string FirstNameFa { get; set; } = string.Empty;
    public string LastNameFa { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? NationalCode { get; set; }
    public string? FatherName { get; set; }
    public string? FatherNationalCode { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? BirthPlace { get; set; }
    public Gender Gender { get; set; }
    public MaritalStatus MaritalStatus { get; set; }
}
