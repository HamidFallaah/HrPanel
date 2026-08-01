using HrPanel.Application.Common.Models;
using HrPanel.Application.Dtos.Organization;
using HrPanel.Domain.Organization;

namespace HrPanel.Application.Common.Abstractions.Persistence;

public interface IOrganizationRepository
{
    Task<PagedResult<OrganizationUnitDto>> GetOrganizationUnitsAsync(GetOrganizationUnitsDto request,CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OrganizationUnitTreeDto>> GetOrganizationTreeAsync(bool includeInactive,CancellationToken cancellationToken = default);
    Task<OrganizationUnit?> GetOrganizationUnitAsync(long id,CancellationToken cancellationToken = default);
    Task<bool> OrganizationUnitTypeExistsAsync(short id,CancellationToken cancellationToken = default);
    Task<bool> OrganizationUnitCodeExistsAsync(string code,long? excludingId = null,CancellationToken cancellationToken = default);
    Task<bool> WouldCreateOrganizationCycleAsync(long unitId,long parentId,CancellationToken cancellationToken = default);

    Task<PagedResult<PositionDto>> GetPositionsAsync(GetOrganizationItemsDto request,CancellationToken cancellationToken = default);
    Task<Position?> GetPositionAsync(long id,CancellationToken cancellationToken = default);
    Task<bool> PositionCodeExistsAsync(string code,long? excludingId = null,CancellationToken cancellationToken = default);

    Task<PagedResult<WorkLocationDto>> GetWorkLocationsAsync(GetOrganizationItemsDto request,CancellationToken cancellationToken = default);
    Task<WorkLocation?> GetWorkLocationAsync(long id,CancellationToken cancellationToken = default);
    Task<bool> WorkLocationCodeExistsAsync(string code,long? excludingId = null,CancellationToken cancellationToken = default);

    Task<PagedResult<OperationalGroupDto>> GetOperationalGroupsAsync(GetOrganizationItemsDto request,CancellationToken cancellationToken = default);
    Task<OperationalGroup?> GetOperationalGroupAsync(long id,CancellationToken cancellationToken = default);
    Task<bool> OperationalGroupCodeExistsAsync(string code,CancellationToken cancellationToken = default);

    void Add(OrganizationUnit unit);
    void Add(Position position);
    void Add(WorkLocation location);
    void Add(OperationalGroup group);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
