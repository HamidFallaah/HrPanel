using HrPanel.Application.Common.Models;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Dtos.Organization;

namespace HrPanel.Application.Features.Organization;

public interface IOrganizationService
{
    Task<Result<PagedResult<OrganizationUnitDto>>> GetOrganizationUnitsAsync(GetOrganizationUnitsDto request,CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<OrganizationUnitTreeDto>>> GetOrganizationTreeAsync(bool includeInactive,CancellationToken cancellationToken = default);
    Task<Result<OrganizationUnitDto>> GetOrganizationUnitAsync(long id,CancellationToken cancellationToken = default);
    Task<Result<long>> CreateOrganizationUnitAsync(CreateOrganizationUnitDto request,CancellationToken cancellationToken = default);
    Task<Result> UpdateOrganizationUnitAsync(long id,UpdateOrganizationUnitDto request,CancellationToken cancellationToken = default);
    Task<Result> MoveOrganizationUnitAsync(long id,MoveOrganizationUnitDto request,CancellationToken cancellationToken = default);
    Task<Result> ChangeOrganizationUnitStatusAsync(long id,bool isActive,CancellationToken cancellationToken = default);

    Task<Result<PagedResult<PositionDto>>> GetPositionsAsync(GetOrganizationItemsDto request,CancellationToken cancellationToken = default);
    Task<Result<PositionDto>> GetPositionAsync(long id,CancellationToken cancellationToken = default);
    Task<Result<long>> CreatePositionAsync(SavePositionDto request,CancellationToken cancellationToken = default);
    Task<Result> UpdatePositionAsync(long id,SavePositionDto request,CancellationToken cancellationToken = default);
    Task<Result> ChangePositionStatusAsync(long id,bool isActive,CancellationToken cancellationToken = default);

    Task<Result<PagedResult<WorkLocationDto>>> GetWorkLocationsAsync(GetOrganizationItemsDto request,CancellationToken cancellationToken = default);
    Task<Result<WorkLocationDto>> GetWorkLocationAsync(long id,CancellationToken cancellationToken = default);
    Task<Result<long>> CreateWorkLocationAsync(SaveWorkLocationDto request,CancellationToken cancellationToken = default);
    Task<Result> UpdateWorkLocationAsync(long id,SaveWorkLocationDto request,CancellationToken cancellationToken = default);
    Task<Result> ChangeWorkLocationStatusAsync(long id,bool isActive,CancellationToken cancellationToken = default);

    Task<Result<PagedResult<OperationalGroupDto>>> GetOperationalGroupsAsync(GetOrganizationItemsDto request,CancellationToken cancellationToken = default);
    Task<Result<OperationalGroupDto>> GetOperationalGroupAsync(long id,CancellationToken cancellationToken = default);
    Task<Result<long>> CreateOperationalGroupAsync(CreateOperationalGroupDto request,CancellationToken cancellationToken = default);
    Task<Result> UpdateOperationalGroupAsync(long id,UpdateOperationalGroupDto request,CancellationToken cancellationToken = default);
    Task<Result> ChangeOperationalGroupStatusAsync(long id,bool isActive,CancellationToken cancellationToken = default);
}
