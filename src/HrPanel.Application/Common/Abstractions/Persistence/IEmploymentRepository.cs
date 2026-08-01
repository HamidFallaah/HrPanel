using HrPanel.Application.Common.Models;
using HrPanel.Application.Dtos.Employments;
using HrPanel.Domain.Employment;
using HrPanel.Domain.Scheduling;

namespace HrPanel.Application.Common.Abstractions.Persistence;

public interface IEmploymentRepository
{
    Task<PagedResult<EmploymentListItemDto>> GetPagedAsync(
        GetEmploymentsDto request,
        CancellationToken cancellationToken = default);

    Task<EmploymentDetailsDto?> GetDetailsAsync(
        long employmentId,
        CancellationToken cancellationToken = default);

    Task<Employment?> GetByIdAsync(
        long employmentId,
        CancellationToken cancellationToken = default);

    Task<bool> EmployeeExistsAsync(long employeeId,CancellationToken cancellationToken = default);
    Task<bool> CurrentEmploymentExistsAsync(long employeeId,CancellationToken cancellationToken = default);
    Task<bool> EmploymentTypeExistsAsync(short id,CancellationToken cancellationToken = default);
    Task<bool> EmploymentStatusExistsAsync(short id,CancellationToken cancellationToken = default);
    Task<bool> WorkTimeTypeExistsAsync(short id,CancellationToken cancellationToken = default);
    Task<bool> AssignmentReferencesExistAsync(AddEmployeeAssignmentDto request,CancellationToken cancellationToken = default);
    Task<bool> OperationalGroupExistsAsync(long id,CancellationToken cancellationToken = default);
    Task<EmployeeOperationalGroupAssignment?> GetOperationalGroupAssignmentAsync(long employmentId,long assignmentId,CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<EmployeeOperationalGroupAssignment>> GetCurrentOperationalGroupAssignmentsAsync(long employmentId,CancellationToken cancellationToken = default);
    Task<EmployeeScheduleAssignment?> GetCurrentScheduleAssignmentAsync(long employmentId,CancellationToken cancellationToken = default);
    Task<EmployeeRelationship?> GetRelationshipAsync(long employeeId,long relationshipId,CancellationToken cancellationToken = default);
    Task<bool> ExternalPersonExistsAsync(long externalPersonId,CancellationToken cancellationToken = default);
    Task<bool> CurrentRelationshipExistsAsync(long employeeId,RelationshipType type,RelationshipContext context,CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ExternalPersonDto>> GetExternalPersonsAsync(string? search,bool? isActive,CancellationToken cancellationToken = default);
    Task<ExternalPerson?> GetExternalPersonAsync(long externalPersonId,CancellationToken cancellationToken = default);
    Task<bool> ExternalUsernameExistsAsync(string legacyUsername,long? excludingId = null,CancellationToken cancellationToken = default);
    Task<DisciplinaryAction?> GetDisciplinaryActionAsync(long employeeId,long actionId,CancellationToken cancellationToken = default);

    void Add(Employment employment);
    void Add(EmployeeOperationalGroupAssignment assignment);
    void Add(EmployeeRelationship relationship);
    void Add(ExternalPerson externalPerson);
    void Add(DisciplinaryAction disciplinaryAction);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
