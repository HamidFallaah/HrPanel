using HrPanel.Application.Common.Models;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Dtos.Employments;

namespace HrPanel.Application.Features.Employments;

public interface IEmploymentService
{
    Task<Result<PagedResult<EmploymentListItemDto>>> GetEmploymentsAsync(GetEmploymentsDto request,CancellationToken cancellationToken = default);
    Task<Result<EmploymentDetailsDto>> GetEmploymentAsync(long employmentId,CancellationToken cancellationToken = default);
    Task<Result<long>> StartEmploymentAsync(StartEmploymentDto request,CancellationToken cancellationToken = default);
    Task<Result> ChangeStatusAsync(long employmentId,ChangeEmploymentStatusDto request,CancellationToken cancellationToken = default);
    Task<Result> ChangeWorkTimeTypeAsync(long employmentId,ChangeWorkTimeTypeDto request,CancellationToken cancellationToken = default);
    Task<Result> EndEmploymentAsync(long employmentId,EndEmploymentDto request,CancellationToken cancellationToken = default);
    Task<Result<long>> AddAssignmentAsync(long employmentId,AddEmployeeAssignmentDto request,CancellationToken cancellationToken = default);
    Task<Result> EndAssignmentAsync(long employmentId,long assignmentId,EndAssignmentDto request,CancellationToken cancellationToken = default);
    Task<Result<long>> AssignOperationalGroupAsync(long employmentId,AssignOperationalGroupDto request,CancellationToken cancellationToken = default);
    Task<Result> SelectPrimaryOperationalGroupAsync(long employmentId,long assignmentId,CancellationToken cancellationToken = default);
    Task<Result> EndOperationalGroupAssignmentAsync(long employmentId,long assignmentId,EndAssignmentDto request,CancellationToken cancellationToken = default);
    Task<Result<long>> AddRelationshipAsync(long employeeId,AddEmployeeRelationshipDto request,CancellationToken cancellationToken = default);
    Task<Result> EndRelationshipAsync(long employeeId,long relationshipId,EndAssignmentDto request,CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<ExternalPersonDto>>> GetExternalPersonsAsync(string? search,bool? isActive,CancellationToken cancellationToken = default);
    Task<Result<long>> CreateExternalPersonAsync(CreateExternalPersonDto request,CancellationToken cancellationToken = default);
    Task<Result> UpdateExternalPersonAsync(long externalPersonId,UpdateExternalPersonDto request,CancellationToken cancellationToken = default);
    Task<Result> ChangeExternalPersonStatusAsync(long externalPersonId,bool isActive,CancellationToken cancellationToken = default);
    Task<Result<long>> AddDisciplinaryActionAsync(AddDisciplinaryActionDto request,CancellationToken cancellationToken = default);
    Task<Result> CloseDisciplinaryActionAsync(long employeeId,long actionId,CloseDisciplinaryActionDto request,CancellationToken cancellationToken = default);
}
