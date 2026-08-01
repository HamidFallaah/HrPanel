using HrPanel.Application.Dtos.Lookups;

namespace HrPanel.Application.Features.Lookups;

public interface IEmployeeLookupService
{
    EmployeeLookupsDto GetEmployeeLookups();
    IReadOnlyCollection<LookupItemDto> GetContactTypes();
    IReadOnlyCollection<LookupItemDto> GetIdentifierTypes();
    IReadOnlyCollection<LookupItemDto> GetGenders();
    IReadOnlyCollection<LookupItemDto> GetMaritalStatuses();
    IReadOnlyCollection<LookupItemDto> GetDependentRelationshipTypes();
}
