using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Application.Dtos.Lookups;
using HrPanel.Domain.Assets;
using HrPanel.Domain.Employment;
using HrPanel.Domain.Organization;
using HrPanel.Domain.Scheduling;

namespace HrPanel.Application.Features.Lookups;

public sealed class LookupService : ILookupService
{
    private readonly ILookupRepository _repository;
    private readonly IEmployeeLookupService _employeeLookupService;

    public LookupService(ILookupRepository repository,IEmployeeLookupService employeeLookupService)
    {
        _repository = repository;
        _employeeLookupService = employeeLookupService;
    }

    public EmployeeLookupsDto GetEmployeeLookups() => _employeeLookupService.GetEmployeeLookups();

    public async Task<EmploymentLookupsDto> GetEmploymentLookupsAsync(CancellationToken cancellationToken = default)
    {
        return new EmploymentLookupsDto(
            await _repository.GetEmploymentTypesAsync(cancellationToken),
            await _repository.GetEmploymentStatusesAsync(cancellationToken),
            await _repository.GetWorkTimeTypesAsync(cancellationToken),
            CreateEnumLookup<AssignmentContext>(ModuleLookupNames.GetDisplayName),
            CreateEnumLookup<RelationshipType>(ModuleLookupNames.GetDisplayName),
            CreateEnumLookup<RelationshipContext>(ModuleLookupNames.GetDisplayName));
    }

    public async Task<OrganizationLookupsDto> GetOrganizationLookupsAsync(CancellationToken cancellationToken = default)
    {
        return new OrganizationLookupsDto(
            await _repository.GetOrganizationUnitTypesAsync(cancellationToken),
            await _repository.GetJobLevelsAsync(cancellationToken),
            await _repository.GetOrganizationUnitsAsync(cancellationToken),
            await _repository.GetPositionsAsync(cancellationToken),
            await _repository.GetWorkLocationsAsync(cancellationToken),
            await _repository.GetOperationalGroupsAsync(cancellationToken),
            CreateEnumLookup<OperationalGroupType>(ModuleLookupNames.GetDisplayName));
    }

    public async Task<SchedulingLookupsDto> GetSchedulingLookupsAsync(CancellationToken cancellationToken = default)
    {
        return new SchedulingLookupsDto(
            await _repository.GetShiftsAsync(cancellationToken),
            await _repository.GetWorkSchedulesAsync(cancellationToken),
            CreateEnumLookup<WorkSchedulePatternType>(ModuleLookupNames.GetDisplayName));
    }

    public async Task<AssetLookupsDto> GetAssetLookupsAsync(CancellationToken cancellationToken = default)
    {
        return new AssetLookupsDto(
            await _repository.GetAssetTypesAsync(cancellationToken),
            CreateEnumLookup<AssetStatus>(ModuleLookupNames.GetDisplayName));
    }

    private static IReadOnlyCollection<LookupItemDto> CreateEnumLookup<TEnum>(Func<TEnum,string> displayName)
        where TEnum : struct,Enum
    {
        return Enum.GetValues<TEnum>()
            .Select(value => new LookupItemDto(Convert.ToInt16(value),value.ToString(),displayName(value)))
            .ToArray();
    }
}
