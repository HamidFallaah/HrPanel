using HrPanel.Application.Common.Models;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Domain.Employees;

namespace HrPanel.Application.Common.Abstractions.Persistence;

public interface IEmployeeRepository
{
    Task<bool> EmployeeNumberExistsAsync(string employeeNumber,CancellationToken cancellationToken = default);
    Task<bool> NationalCodeExistsAsync(string nationalCode,long? excludingEmployeeId = null,CancellationToken cancellationToken = default);
    Task<bool> ActiveIdentifierExistsAsync(IdentifierType type,string value,CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdAsync(long employeeId,CancellationToken cancellationToken = default);
    Task<PagedResult<EmployeeListItemDto>> GetPagedAsync(GetEmployeesDto request,CancellationToken cancellationToken = default);
    void Add(Employee employee);
    void UpdatePersonalDetails(EmployeePersonalDetails personalDetails);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
