namespace HrPanel.Application.Dtos.Lookups;

public sealed record EmployeeLookupsDto(
    IReadOnlyCollection<LookupItemDto> ContactTypes,
    IReadOnlyCollection<LookupItemDto> IdentifierTypes,
    IReadOnlyCollection<LookupItemDto> Genders,
    IReadOnlyCollection<LookupItemDto> MaritalStatuses,
    IReadOnlyCollection<LookupItemDto> DependentRelationshipTypes);
