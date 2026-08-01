namespace HrPanel.Application.Dtos.Lookups;

public sealed record ReferenceLookupItemDto(long Id,string Code,string DisplayName,string? EnglishName = null);

public sealed record EmploymentLookupsDto(
    IReadOnlyCollection<ReferenceLookupItemDto> EmploymentTypes,
    IReadOnlyCollection<ReferenceLookupItemDto> EmploymentStatuses,
    IReadOnlyCollection<ReferenceLookupItemDto> WorkTimeTypes,
    IReadOnlyCollection<LookupItemDto> AssignmentContexts,
    IReadOnlyCollection<LookupItemDto> RelationshipTypes,
    IReadOnlyCollection<LookupItemDto> RelationshipContexts);

public sealed record OrganizationLookupsDto(
    IReadOnlyCollection<ReferenceLookupItemDto> OrganizationUnitTypes,
    IReadOnlyCollection<ReferenceLookupItemDto> JobLevels,
    IReadOnlyCollection<ReferenceLookupItemDto> OrganizationUnits,
    IReadOnlyCollection<ReferenceLookupItemDto> Positions,
    IReadOnlyCollection<ReferenceLookupItemDto> WorkLocations,
    IReadOnlyCollection<ReferenceLookupItemDto> OperationalGroups,
    IReadOnlyCollection<LookupItemDto> OperationalGroupTypes);

public sealed record SchedulingLookupsDto(IReadOnlyCollection<ReferenceLookupItemDto> Shifts,IReadOnlyCollection<ReferenceLookupItemDto> WorkSchedules,IReadOnlyCollection<LookupItemDto> WorkSchedulePatternTypes);

public sealed record AssetLookupsDto(IReadOnlyCollection<ReferenceLookupItemDto> AssetTypes,IReadOnlyCollection<LookupItemDto> AssetStatuses);
