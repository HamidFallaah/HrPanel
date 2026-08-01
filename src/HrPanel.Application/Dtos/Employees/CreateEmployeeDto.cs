namespace HrPanel.Application.Dtos.Employees;

public sealed record CreateEmployeeDto(string EmployeeNumber,string FirstNameFa,string LastNameFa,string? FirstName,string? LastName,string? NationalCode);
