using HrPanel.Application.Common.Models;
using HrPanel.Application.Dtos.Assets;
using HrPanel.Domain.Assets;

namespace HrPanel.Application.Common.Abstractions.Persistence;

public interface IAssetRepository
{
    Task<PagedResult<AssetListItemDto>> GetPagedAsync(GetAssetsDto request,CancellationToken cancellationToken = default);
    Task<AssetDetailsDto?> GetDetailsAsync(long id,CancellationToken cancellationToken = default);
    Task<Asset?> GetByIdAsync(long id,CancellationToken cancellationToken = default);
    Task<bool> AssetTypeExistsAsync(short id,CancellationToken cancellationToken = default);
    Task<bool> EmployeeExistsAsync(long employeeId,CancellationToken cancellationToken = default);
    Task<bool> IdentifierExistsAsync(string propertyName,string value,long? excludingId = null,CancellationToken cancellationToken = default);
    Task<EmployeeAssetAssignment?> GetCurrentAssignmentAsync(long assetId,CancellationToken cancellationToken = default);

    void Add(Asset asset);
    void Add(EmployeeAssetAssignment assignment);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
