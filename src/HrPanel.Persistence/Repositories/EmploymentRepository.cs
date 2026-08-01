using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Application.Common.Models;
using HrPanel.Application.Dtos.Employments;
using HrPanel.Domain.Employment;
using HrPanel.Domain.Scheduling;
using HrPanel.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using EmploymentEntity = HrPanel.Domain.Employment.Employment;

namespace HrPanel.Persistence.Repositories;

public sealed class EmploymentRepository : IEmploymentRepository
{
    private readonly HrDbContext _dbContext;

    public EmploymentRepository(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<EmploymentListItemDto>> GetPagedAsync(GetEmploymentsDto request,CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Employments.AsNoTracking();
        if (request.EmployeeId.HasValue) query = query.Where(item => item.EmployeeId == request.EmployeeId.Value);
        if (request.IsCurrent.HasValue) query = request.IsCurrent.Value ? query.Where(item => item.EndDate == null) : query.Where(item => item.EndDate != null);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(item => item.Employee.EmployeeNumber.Contains(search) || (item.Employee.PersonalDetails != null && (item.Employee.PersonalDetails.FirstNameFa.Contains(search) || item.Employee.PersonalDetails.LastNameFa.Contains(search) || (item.Employee.PersonalDetails.NationalCode != null && item.Employee.PersonalDetails.NationalCode.Contains(search)))));
        }
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(item => item.EndDate == null).ThenBy(item => item.Employee.EmployeeNumber)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(item => new EmploymentListItemDto(
                item.Id,item.EmployeeId,item.Employee.EmployeeNumber,
                item.Employee.PersonalDetails == null ? item.Employee.EmployeeNumber : item.Employee.PersonalDetails.FirstNameFa + " " + item.Employee.PersonalDetails.LastNameFa,
                item.EmploymentTypeId,item.EmploymentType.NameFa,item.EmploymentStatusId,item.EmploymentStatus.NameFa,
                item.WorkTimeTypeId,item.WorkTimeType == null ? null : item.WorkTimeType.NameFa,item.StartDate,item.EndDate,item.EndDate == null))
            .ToListAsync(cancellationToken);
        return PagedResult<EmploymentListItemDto>.Create(items,request.PageNumber,request.PageSize,totalCount);
    }

    public async Task<EmploymentDetailsDto?> GetDetailsAsync(long employmentId,CancellationToken cancellationToken = default)
    {
        var header = await _dbContext.Employments.AsNoTracking().Where(item => item.Id == employmentId)
            .Select(item => new EmploymentHeader(
                item.Id,item.EmployeeId,item.Employee.EmployeeNumber,
                item.Employee.PersonalDetails == null ? item.Employee.EmployeeNumber : item.Employee.PersonalDetails.FirstNameFa + " " + item.Employee.PersonalDetails.LastNameFa,
                item.EmploymentTypeId,item.EmploymentType.Code,item.EmploymentType.NameFa,item.EmploymentStatusId,item.EmploymentStatus.Code,item.EmploymentStatus.NameFa,
                item.WorkTimeTypeId,item.WorkTimeType == null ? null : item.WorkTimeType.Code,item.WorkTimeType == null ? null : item.WorkTimeType.NameFa,
                item.StartDate,item.EndDate,item.ContractTermMonths,item.TerminationReason,item.CreatedAt,item.ModifiedAt))
            .SingleOrDefaultAsync(cancellationToken);
        if (header is null) return null;

        var assignments = await _dbContext.EmployeeAssignments.AsNoTracking().Where(item => item.EmploymentId == employmentId).OrderByDescending(item => item.EffectiveFrom)
            .Select(item => new EmployeeAssignmentDto(item.Id,item.Context,item.OrganizationUnitId,item.OrganizationUnit == null ? null : item.OrganizationUnit.NameFa,item.PositionId,item.Position == null ? null : item.Position.TitleFa,item.JobLevelId,item.JobLevel == null ? null : item.JobLevel.TitleFa,item.WorkLocationId,item.WorkLocation == null ? null : item.WorkLocation.NameFa,item.EffectiveFrom,item.EffectiveTo,item.EffectiveTo == null))
            .ToListAsync(cancellationToken);

        var groups = await _dbContext.EmployeeOperationalGroupAssignments.AsNoTracking().Where(item => item.EmploymentId == employmentId).OrderByDescending(item => item.EffectiveFrom)
            .Select(item => new OperationalGroupAssignmentDto(item.Id,item.OperationalGroupId,item.OperationalGroup.Code,item.OperationalGroup.Name,item.EffectiveFrom,item.EffectiveTo,item.IsPrimary,item.EffectiveTo == null))
            .ToListAsync(cancellationToken);

        var relationships = await _dbContext.EmployeeRelationships.AsNoTracking().Where(item => item.EmployeeId == header.EmployeeId).OrderByDescending(item => item.EffectiveFrom)
            .Select(item => new EmployeeRelationshipDto(
                item.Id,item.Type,item.Context,item.RelatedEmployeeId,item.RelatedExternalPersonId,
                item.RelatedEmployee != null ? (item.RelatedEmployee.PersonalDetails == null ? item.RelatedEmployee.EmployeeNumber : item.RelatedEmployee.PersonalDetails.FirstNameFa + " " + item.RelatedEmployee.PersonalDetails.LastNameFa) : item.RelatedExternalPerson!.DisplayName,
                item.EffectiveFrom,item.EffectiveTo,item.EffectiveTo == null))
            .ToListAsync(cancellationToken);

        var disciplinaryActions = await _dbContext.DisciplinaryActions.AsNoTracking().Where(item => item.EmployeeId == header.EmployeeId).OrderByDescending(item => item.StartDate)
            .Select(item => new DisciplinaryActionDto(item.Id,item.EmployeeId,item.StartDate,item.EndDate,item.Details,item.EndDate != null,item.CreatedAt,item.ModifiedAt))
            .ToListAsync(cancellationToken);

        return new EmploymentDetailsDto(
            header.Id,header.EmployeeId,header.EmployeeNumber,header.EmployeeDisplayName,
            header.EmploymentTypeId,header.EmploymentTypeCode,header.EmploymentTypeName,
            header.EmploymentStatusId,header.EmploymentStatusCode,header.EmploymentStatusName,
            header.WorkTimeTypeId,header.WorkTimeTypeCode,header.WorkTimeTypeName,
            header.StartDate,header.EndDate,header.ContractTermMonths,header.TerminationReason,header.EndDate is null,
            assignments,groups,relationships,disciplinaryActions,header.CreatedAt,header.ModifiedAt);
    }

    public Task<EmploymentEntity?> GetByIdAsync(long employmentId,CancellationToken cancellationToken = default) => _dbContext.Employments.Include(item => item.Assignments).SingleOrDefaultAsync(item => item.Id == employmentId,cancellationToken);
    public Task<bool> EmployeeExistsAsync(long employeeId,CancellationToken cancellationToken = default) => _dbContext.Employees.AnyAsync(item => item.Id == employeeId,cancellationToken);
    public Task<bool> CurrentEmploymentExistsAsync(long employeeId,CancellationToken cancellationToken = default) => _dbContext.Employments.AnyAsync(item => item.EmployeeId == employeeId && item.EndDate == null,cancellationToken);
    public Task<bool> EmploymentTypeExistsAsync(short id,CancellationToken cancellationToken = default) => _dbContext.EmploymentTypes.AnyAsync(item => item.Id == id && item.IsActive,cancellationToken);
    public Task<bool> EmploymentStatusExistsAsync(short id,CancellationToken cancellationToken = default) => _dbContext.EmploymentStatuses.AnyAsync(item => item.Id == id && item.IsActive,cancellationToken);
    public Task<bool> WorkTimeTypeExistsAsync(short id,CancellationToken cancellationToken = default) => _dbContext.WorkTimeTypes.AnyAsync(item => item.Id == id && item.IsActive,cancellationToken);

    public async Task<bool> AssignmentReferencesExistAsync(AddEmployeeAssignmentDto request,CancellationToken cancellationToken = default)
    {
        if (request.OrganizationUnitId.HasValue && !await _dbContext.OrganizationUnits.AnyAsync(item => item.Id == request.OrganizationUnitId.Value && item.IsActive,cancellationToken)) return false;
        if (request.PositionId.HasValue && !await _dbContext.Positions.AnyAsync(item => item.Id == request.PositionId.Value && item.IsActive,cancellationToken)) return false;
        if (request.JobLevelId.HasValue && !await _dbContext.JobLevels.AnyAsync(item => item.Id == request.JobLevelId.Value && item.IsActive,cancellationToken)) return false;
        if (request.WorkLocationId.HasValue && !await _dbContext.WorkLocations.AnyAsync(item => item.Id == request.WorkLocationId.Value && item.IsActive,cancellationToken)) return false;
        return true;
    }

    public Task<bool> OperationalGroupExistsAsync(long id,CancellationToken cancellationToken = default) => _dbContext.OperationalGroups.AnyAsync(item => item.Id == id && item.IsActive,cancellationToken);
    public Task<EmployeeOperationalGroupAssignment?> GetOperationalGroupAssignmentAsync(long employmentId,long assignmentId,CancellationToken cancellationToken = default) => _dbContext.EmployeeOperationalGroupAssignments.SingleOrDefaultAsync(item => item.EmploymentId == employmentId && item.Id == assignmentId,cancellationToken);
    public async Task<IReadOnlyCollection<EmployeeOperationalGroupAssignment>> GetCurrentOperationalGroupAssignmentsAsync(long employmentId,CancellationToken cancellationToken = default) => await _dbContext.EmployeeOperationalGroupAssignments.Where(item => item.EmploymentId == employmentId && item.EffectiveTo == null).ToListAsync(cancellationToken);
    public Task<EmployeeScheduleAssignment?> GetCurrentScheduleAssignmentAsync(long employmentId,CancellationToken cancellationToken = default) => _dbContext.EmployeeScheduleAssignments.SingleOrDefaultAsync(item => item.EmploymentId == employmentId && item.EffectiveTo == null,cancellationToken);
    public Task<EmployeeRelationship?> GetRelationshipAsync(long employeeId,long relationshipId,CancellationToken cancellationToken = default) => _dbContext.EmployeeRelationships.SingleOrDefaultAsync(item => item.EmployeeId == employeeId && item.Id == relationshipId,cancellationToken);
    public Task<bool> ExternalPersonExistsAsync(long externalPersonId,CancellationToken cancellationToken = default) => _dbContext.ExternalPersons.AnyAsync(item => item.Id == externalPersonId && item.IsActive,cancellationToken);
    public Task<bool> CurrentRelationshipExistsAsync(long employeeId,RelationshipType type,RelationshipContext context,CancellationToken cancellationToken = default) => _dbContext.EmployeeRelationships.AnyAsync(item => item.EmployeeId == employeeId && item.Type == type && item.Context == context && item.EffectiveTo == null,cancellationToken);

    public async Task<IReadOnlyCollection<ExternalPersonDto>> GetExternalPersonsAsync(string? search,bool? isActive,CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ExternalPersons.AsNoTracking();
        if (isActive.HasValue) query = query.Where(item => item.IsActive == isActive.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();
            query = query.Where(item => item.DisplayName.Contains(value) || (item.LegacyUsername != null && item.LegacyUsername.Contains(value)));
        }
        return await query.OrderBy(item => item.DisplayName).Select(item => new ExternalPersonDto(item.Id,item.DisplayName,item.LegacyUsername,item.IsActive,item.CreatedAt,item.ModifiedAt)).ToListAsync(cancellationToken);
    }

    public Task<ExternalPerson?> GetExternalPersonAsync(long externalPersonId,CancellationToken cancellationToken = default) => _dbContext.ExternalPersons.SingleOrDefaultAsync(item => item.Id == externalPersonId,cancellationToken);
    public Task<bool> ExternalUsernameExistsAsync(string legacyUsername,long? excludingId = null,CancellationToken cancellationToken = default) => _dbContext.ExternalPersons.AnyAsync(item => item.LegacyUsername == legacyUsername && (!excludingId.HasValue || item.Id != excludingId.Value),cancellationToken);
    public Task<DisciplinaryAction?> GetDisciplinaryActionAsync(long employeeId,long actionId,CancellationToken cancellationToken = default) => _dbContext.DisciplinaryActions.SingleOrDefaultAsync(item => item.EmployeeId == employeeId && item.Id == actionId,cancellationToken);

    public void Add(EmploymentEntity employment) => _dbContext.Employments.Add(employment);
    public void Add(EmployeeOperationalGroupAssignment assignment) => _dbContext.EmployeeOperationalGroupAssignments.Add(assignment);
    public void Add(EmployeeRelationship relationship) => _dbContext.EmployeeRelationships.Add(relationship);
    public void Add(ExternalPerson externalPerson) => _dbContext.ExternalPersons.Add(externalPerson);
    public void Add(DisciplinaryAction disciplinaryAction) => _dbContext.DisciplinaryActions.Add(disciplinaryAction);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _dbContext.SaveChangesAsync(cancellationToken);

    private sealed record EmploymentHeader(
        long Id,long EmployeeId,string EmployeeNumber,string EmployeeDisplayName,
        short EmploymentTypeId,string EmploymentTypeCode,string EmploymentTypeName,
        short EmploymentStatusId,string EmploymentStatusCode,string EmploymentStatusName,
        short? WorkTimeTypeId,string? WorkTimeTypeCode,string? WorkTimeTypeName,
        DateOnly StartDate,DateOnly? EndDate,short? ContractTermMonths,string? TerminationReason,
        DateTime CreatedAt,DateTime? ModifiedAt);
}
