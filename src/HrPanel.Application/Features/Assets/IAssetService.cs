using HrPanel.Application.Common.Models;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Dtos.Assets;

namespace HrPanel.Application.Features.Assets;

public interface IAssetService
{
    Task<Result<PagedResult<AssetListItemDto>>> GetAssetsAsync(GetAssetsDto request,CancellationToken cancellationToken = default);
    Task<Result<AssetDetailsDto>> GetAssetAsync(long id,CancellationToken cancellationToken = default);
    Task<Result<long>> CreateAssetAsync(CreateAssetDto request,CancellationToken cancellationToken = default);
    Task<Result> UpdateAssetAsync(long id,UpdateAssetDto request,CancellationToken cancellationToken = default);
    Task<Result<long>> AssignAssetAsync(long id,AssignAssetDto request,CancellationToken cancellationToken = default);
    Task<Result> ReturnAssetAsync(long id,ReturnAssetDto request,CancellationToken cancellationToken = default);
    Task<Result> SendToMaintenanceAsync(long id,CancellationToken cancellationToken = default);
    Task<Result> RetireAssetAsync(long id,CancellationToken cancellationToken = default);
    Task<Result> MarkAssetAsLostAsync(long id,CancellationToken cancellationToken = default);
}
