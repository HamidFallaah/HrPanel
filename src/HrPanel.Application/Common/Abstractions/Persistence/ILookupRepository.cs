using HrPanel.Application.Dtos.Lookups;

namespace HrPanel.Application.Common.Abstractions.Persistence;

public interface ILookupRepository
{
    Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetEmploymentTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetEmploymentStatusesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetWorkTimeTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetOrganizationUnitTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetJobLevelsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetOrganizationUnitsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetPositionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetWorkLocationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetOperationalGroupsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetShiftsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetWorkSchedulesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetAssetTypesAsync(CancellationToken cancellationToken = default);
}
