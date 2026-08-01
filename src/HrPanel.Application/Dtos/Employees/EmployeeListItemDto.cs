namespace HrPanel.Application.Dtos.Employees;

public sealed record EmployeeListItemDto(long Id,string EmployeeNumber,string? FirstName,string? LastName,string? FirstNameFa,string? LastNameFa,string? NationalCode,bool IsActive);
