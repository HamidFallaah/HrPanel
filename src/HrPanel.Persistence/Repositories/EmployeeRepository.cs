using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Application.Common.Models;
using HrPanel.Application.Dtos.Employees;
using HrPanel.Domain.Employees;
using HrPanel.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace HrPanel.Persistence.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly HrDbContext _dbContext;
    public EmployeeRepository(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<bool> EmployeeNumberExistsAsync(string employeeNumber,CancellationToken cancellationToken = default)
    {
        return _dbContext.Employees.AnyAsync(employee => employee.EmployeeNumber == employeeNumber,cancellationToken);
    }
    public Task<bool> NationalCodeExistsAsync(string nationalCode,long? excludingEmployeeId = null,CancellationToken cancellationToken = default)
    {
        return _dbContext.EmployeePersonalDetails.AnyAsync(personalDetails => personalDetails.NationalCode == nationalCode &&(!excludingEmployeeId.HasValue ||personalDetails.EmployeeId != excludingEmployeeId.Value),cancellationToken);
    }
    public Task<bool> ActiveIdentifierExistsAsync(IdentifierType type,string value,CancellationToken cancellationToken = default)
    {
        return _dbContext.EmployeeIdentifiers.AnyAsync(identifier =>identifier.Type == type &&identifier.Value == value &&identifier.EffectiveTo == null,cancellationToken);
    }
    public Task<Employee?> GetByIdAsync(long employeeId,CancellationToken cancellationToken = default)
    {
        return _dbContext.Employees
            .AsSplitQuery()
            .Include(employee => employee.PersonalDetails)
            .Include(employee => employee.Contacts)
            .Include(employee => employee.Identifiers)
            .Include(employee => employee.EducationRecords)
            .Include(employee => employee.Dependents)
            .SingleOrDefaultAsync(
                employee => employee.Id == employeeId,
                cancellationToken);
    }

    public async Task<PagedResult<EmployeeListItemDto>> GetPagedAsync(GetEmployeesDto request,CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Employees.AsNoTracking();

        if (request.IsActive.HasValue)
        {
            query = query.Where(
                employee => employee.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(employee =>
                employee.EmployeeNumber.Contains(search) ||
                (employee.PersonalDetails != null &&
                 (employee.PersonalDetails.FirstNameFa.Contains(search) ||
                  employee.PersonalDetails.LastNameFa.Contains(search) ||
                  (employee.PersonalDetails.NationalCode != null &&
                   employee.PersonalDetails.NationalCode.Contains(search)) ||
                  (employee.PersonalDetails.FirstName != null &&
                   employee.PersonalDetails.FirstName.Contains(search)) ||
                  (employee.PersonalDetails.LastName != null &&
                   employee.PersonalDetails.LastName.Contains(search)))));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var employees = await query
            .OrderBy(employee => employee.EmployeeNumber)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(employee => new EmployeeListItemDto(
                employee.Id,
                employee.EmployeeNumber,
                employee.PersonalDetails == null
                    ? null
                    : employee.PersonalDetails.FirstName,
                employee.PersonalDetails == null
                    ? null
                    : employee.PersonalDetails.LastName,
                employee.PersonalDetails == null
                    ? null
                    : employee.PersonalDetails.FirstNameFa,
                employee.PersonalDetails == null
                    ? null
                    : employee.PersonalDetails.LastNameFa,
                employee.PersonalDetails == null
                    ? null
                    : employee.PersonalDetails.NationalCode,
                employee.IsActive))
            .ToListAsync(cancellationToken);

        return PagedResult<EmployeeListItemDto>.Create(employees, request.PageNumber,request.PageSize,totalCount);
    }

    public void Add(Employee employee)
    {
        _dbContext.Employees.Add(employee);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
