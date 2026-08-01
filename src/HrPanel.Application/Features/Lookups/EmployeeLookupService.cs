using HrPanel.Application.Dtos.Lookups;
using HrPanel.Domain.Employees;

namespace HrPanel.Application.Features.Lookups;

public sealed class EmployeeLookupService : IEmployeeLookupService
{
    public EmployeeLookupsDto GetEmployeeLookups()
    {
        return new EmployeeLookupsDto(
            GetContactTypes(),
            GetIdentifierTypes(),
            GetGenders(),
            GetMaritalStatuses(),
            GetDependentRelationshipTypes());
    }

    public IReadOnlyCollection<LookupItemDto> GetContactTypes()
    {
        return CreateLookup<ContactType>(EmployeeLookupNames.GetDisplayName);
    }
    public IReadOnlyCollection<LookupItemDto> GetIdentifierTypes()
    {
        return CreateLookup<IdentifierType>(EmployeeLookupNames.GetDisplayName);
    }
    public IReadOnlyCollection<LookupItemDto> GetGenders()
    {
        return CreateLookup<Gender>(EmployeeLookupNames.GetDisplayName);
    }
    public IReadOnlyCollection<LookupItemDto> GetMaritalStatuses()
    {
        return CreateLookup<MaritalStatus>(EmployeeLookupNames.GetDisplayName);
    }
    public IReadOnlyCollection<LookupItemDto> GetDependentRelationshipTypes()
    {
        return CreateLookup<DependentRelationshipType>(EmployeeLookupNames.GetDisplayName);
    }
    private static IReadOnlyCollection<LookupItemDto> CreateLookup<TEnum>(Func<TEnum,string> getDisplayName)where TEnum : struct,Enum
    {
        return Enum.GetValues<TEnum>().Select(value => new LookupItemDto(Convert.ToInt16(value),value.ToString(),getDisplayName(value))).ToArray();
    }
}
