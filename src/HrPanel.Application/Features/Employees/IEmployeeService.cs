using HrPanel.Application.Common.Models;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Dtos.Employees;

namespace HrPanel.Application.Features.Employees;

public interface IEmployeeService
{
    Task<Result<PagedResult<EmployeeListItemDto>>> GetEmployeesAsync(
        GetEmployeesDto request,
        CancellationToken cancellationToken = default);

    Task<Result<long>> CreateEmployeeAsync(
        CreateEmployeeDto request,
        CancellationToken cancellationToken = default);

    Task<Result<EmployeeDetailsDto>> GetEmployeeDetailsAsync(
        long employeeId,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateEmployeeNumberAsync(
        long employeeId,
        UpdateEmployeeNumberDto request,
        CancellationToken cancellationToken = default);

    Task<Result> UpdatePersonalDetailsAsync(
        long employeeId,
        UpdateEmployeePersonalDetailsDto request,
        CancellationToken cancellationToken = default);

    Task<Result> ActivateEmployeeAsync(
        long employeeId,
        CancellationToken cancellationToken = default);

    Task<Result> DeactivateEmployeeAsync(
        long employeeId,
        CancellationToken cancellationToken = default);

    Task<Result<long>> AddContactAsync(
        long employeeId,
        AddEmployeeContactDto request,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateContactAsync(
        long employeeId,
        long contactId,
        UpdateEmployeeContactDto request,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveContactAsync(
        long employeeId,
        long contactId,
        CancellationToken cancellationToken = default);

    Task<Result> SelectPrimaryContactAsync(
        long employeeId,
        long contactId,
        CancellationToken cancellationToken = default);

    Task<Result<long>> AddIdentifierAsync(
        long employeeId,
        AddEmployeeIdentifierDto request,
        CancellationToken cancellationToken = default);

    Task<Result> EndIdentifierAsync(
        long employeeId,
        long identifierId,
        EndEmployeeIdentifierDto request,
        CancellationToken cancellationToken = default);

    Task<Result<long>> AddEducationAsync(
        long employeeId,
        AddEmployeeEducationDto request,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateEducationAsync(
        long employeeId,
        long educationId,
        AddEmployeeEducationDto request,
        CancellationToken cancellationToken = default);

    Task<Result> SelectHighestEducationAsync(
        long employeeId,
        long educationId,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveEducationAsync(
        long employeeId,
        long educationId,
        CancellationToken cancellationToken = default);

    Task<Result<long>> AddDependentAsync(
        long employeeId,
        AddEmployeeDependentDto request,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateDependentAsync(
        long employeeId,
        long dependentId,
        AddEmployeeDependentDto request,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveDependentAsync(
        long employeeId,
        long dependentId,
        CancellationToken cancellationToken = default);
}
