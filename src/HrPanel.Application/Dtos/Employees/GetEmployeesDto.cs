namespace HrPanel.Application.Dtos.Employees;

public sealed record GetEmployeesDto(string? Search = null,bool? IsActive = null,int PageNumber = 1,int PageSize = 20);
