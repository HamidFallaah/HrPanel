using HrPanel.Application.Dtos.Lookups;

namespace HrPanel.Application.Features.Lookups;

public interface ILookupService
{
    EmployeeLookupsDto GetEmployeeLookups();
    Task<EmploymentLookupsDto> GetEmploymentLookupsAsync(CancellationToken cancellationToken = default);
    Task<OrganizationLookupsDto> GetOrganizationLookupsAsync(CancellationToken cancellationToken = default);
    Task<SchedulingLookupsDto> GetSchedulingLookupsAsync(CancellationToken cancellationToken = default);
    Task<AssetLookupsDto> GetAssetLookupsAsync(CancellationToken cancellationToken = default);
}
