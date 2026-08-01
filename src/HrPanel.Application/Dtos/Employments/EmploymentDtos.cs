using HrPanel.Domain.Employment;

namespace HrPanel.Application.Dtos.Employments;

public sealed record GetEmploymentsDto(string? Search = null,long? EmployeeId = null,bool? IsCurrent = null,int PageNumber = 1,int PageSize = 20);

public sealed class StartEmploymentDto
{
    public long EmployeeId { get; set; }
    public short EmploymentTypeId { get; set; }
    public short EmploymentStatusId { get; set; }
    public short? WorkTimeTypeId { get; set; }
    public DateOnly StartDate { get; set; }
    public short? ContractTermMonths { get; set; }
}
public sealed class ChangeEmploymentStatusDto
{
    public short EmploymentStatusId { get; set; }
}
public sealed class ChangeWorkTimeTypeDto
{
    public short? WorkTimeTypeId { get; set; }
}
public sealed class EndEmploymentDto
{
    public DateOnly EndDate { get; set; }
    public short EmploymentStatusId { get; set; }
    public string? Reason { get; set; }
}
public sealed class AddEmployeeAssignmentDto
{
    public AssignmentContext Context { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public long? OrganizationUnitId { get; set; }
    public long? PositionId { get; set; }
    public short? JobLevelId { get; set; }
    public long? WorkLocationId { get; set; }
}
public sealed class EndAssignmentDto
{
    public DateOnly EffectiveTo { get; set; }
}
public sealed class AssignOperationalGroupDto
{
    public long OperationalGroupId { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public bool IsPrimary { get; set; } = true;
}
public sealed class AddEmployeeRelationshipDto
{
    public RelationshipType Type { get; set; }
    public RelationshipContext Context { get; set; }
    public long? RelatedEmployeeId { get; set; }
    public long? RelatedExternalPersonId { get; set; }
    public DateOnly EffectiveFrom { get; set; }
}
public sealed class CreateExternalPersonDto
{
    public string DisplayName { get; set; } = string.Empty;
    public string? LegacyUsername { get; set; }
}
public sealed class UpdateExternalPersonDto
{
    public string DisplayName { get; set; } = string.Empty;
    public string? LegacyUsername { get; set; }
}
public sealed class AddDisciplinaryActionDto
{
    public long EmployeeId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string Details { get; set; } = string.Empty;
}
public sealed class CloseDisciplinaryActionDto
{
    public DateOnly EndDate { get; set; }
}
public sealed record EmploymentListItemDto(
    long Id,
    long EmployeeId,
    string EmployeeNumber,
    string EmployeeDisplayName,
    short EmploymentTypeId,
    string EmploymentTypeName,
    short EmploymentStatusId,
    string EmploymentStatusName,
    short? WorkTimeTypeId,
    string? WorkTimeTypeName,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsCurrent);

public sealed record EmploymentDetailsDto(
    long Id,
    long EmployeeId,
    string EmployeeNumber,
    string EmployeeDisplayName,
    short EmploymentTypeId,
    string EmploymentTypeCode,
    string EmploymentTypeName,
    short EmploymentStatusId,
    string EmploymentStatusCode,
    string EmploymentStatusName,
    short? WorkTimeTypeId,
    string? WorkTimeTypeCode,
    string? WorkTimeTypeName,
    DateOnly StartDate,
    DateOnly? EndDate,
    short? ContractTermMonths,
    string? TerminationReason,
    bool IsCurrent,
    IReadOnlyCollection<EmployeeAssignmentDto> Assignments,
    IReadOnlyCollection<OperationalGroupAssignmentDto> OperationalGroups,
    IReadOnlyCollection<EmployeeRelationshipDto> Relationships,
    IReadOnlyCollection<DisciplinaryActionDto> DisciplinaryActions,
    DateTime CreatedAt,
    DateTime? ModifiedAt);

public sealed record EmployeeAssignmentDto(
    long Id,
    AssignmentContext Context,
    long? OrganizationUnitId,
    string? OrganizationUnitName,
    long? PositionId,
    string? PositionTitle,
    short? JobLevelId,
    string? JobLevelTitle,
    long? WorkLocationId,
    string? WorkLocationName,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    bool IsCurrent);

public sealed record OperationalGroupAssignmentDto(
    long Id,
    long OperationalGroupId,
    string OperationalGroupCode,
    string OperationalGroupName,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    bool IsPrimary,
    bool IsCurrent);

public sealed record EmployeeRelationshipDto(
    long Id,
    RelationshipType Type,
    RelationshipContext Context,
    long? RelatedEmployeeId,
    long? RelatedExternalPersonId,
    string RelatedPersonDisplayName,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    bool IsCurrent);

public sealed record DisciplinaryActionDto(
    long Id,
    long EmployeeId,
    DateOnly StartDate,
    DateOnly? EndDate,
    string Details,
    bool IsClosed,
    DateTime CreatedAt,
    DateTime? ModifiedAt);

public sealed record ExternalPersonDto(
    long Id,
    string DisplayName,
    string? LegacyUsername,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? ModifiedAt);
