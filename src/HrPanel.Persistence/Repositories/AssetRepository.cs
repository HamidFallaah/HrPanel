using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Application.Common.Models;
using HrPanel.Application.Dtos.Assets;
using HrPanel.Domain.Assets;
using HrPanel.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace HrPanel.Persistence.Repositories;

public sealed class AssetRepository : IAssetRepository
{
    private readonly HrDbContext _dbContext;

    public AssetRepository(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<AssetListItemDto>> GetPagedAsync(GetAssetsDto request,CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Assets.AsNoTracking();
        if (request.AssetTypeId.HasValue) query = query.Where(item => item.AssetTypeId == request.AssetTypeId.Value);
        if (request.Status.HasValue) query = query.Where(item => item.Status == request.Status.Value);
        if (request.EmployeeId.HasValue) query = query.Where(item => _dbContext.EmployeeAssetAssignments.Any(assignment => assignment.AssetId == item.Id && assignment.EmployeeId == request.EmployeeId.Value && assignment.ReturnedAt == null));
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(item => (item.AssetTag != null && item.AssetTag.Contains(search)) || (item.ServiceNumber != null && item.ServiceNumber.Contains(search)) || (item.Imei != null && item.Imei.Contains(search)) || (item.SerialNumber != null && item.SerialNumber.Contains(search)));
        }
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(item => item.AssetTypeId).ThenBy(item => item.Id)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(item => new AssetListItemDto(
                item.Id,item.AssetTypeId,item.AssetType.NameFa,item.AssetTag,item.ServiceNumber,item.Imei,item.SerialNumber,item.Status,
                _dbContext.EmployeeAssetAssignments.Where(assignment => assignment.AssetId == item.Id && assignment.ReturnedAt == null).Select(assignment => (long?)assignment.EmployeeId).FirstOrDefault(),
                _dbContext.EmployeeAssetAssignments.Where(assignment => assignment.AssetId == item.Id && assignment.ReturnedAt == null).Select(assignment => assignment.Employee.EmployeeNumber).FirstOrDefault(),
                _dbContext.EmployeeAssetAssignments.Where(assignment => assignment.AssetId == item.Id && assignment.ReturnedAt == null).Select(assignment => assignment.Employee.PersonalDetails == null ? assignment.Employee.EmployeeNumber : assignment.Employee.PersonalDetails.FirstNameFa + " " + assignment.Employee.PersonalDetails.LastNameFa).FirstOrDefault()))
            .ToListAsync(cancellationToken);
        return PagedResult<AssetListItemDto>.Create(items,request.PageNumber,request.PageSize,totalCount);
    }

    public Task<AssetDetailsDto?> GetDetailsAsync(long id,CancellationToken cancellationToken = default)
    {
        return _dbContext.Assets.AsNoTracking().Where(item => item.Id == id)
            .Select(item => new AssetDetailsDto(
                item.Id,item.AssetTypeId,item.AssetType.Code,item.AssetType.NameFa,item.AssetTag,item.ServiceNumber,item.Imei,item.SerialNumber,item.Status,item.Notes,
                _dbContext.EmployeeAssetAssignments.Where(assignment => assignment.AssetId == item.Id).OrderByDescending(assignment => assignment.AssignedAt)
                    .Select(assignment => new AssetAssignmentDto(assignment.Id,assignment.EmployeeId,assignment.Employee.EmployeeNumber,assignment.Employee.PersonalDetails == null ? assignment.Employee.EmployeeNumber : assignment.Employee.PersonalDetails.FirstNameFa + " " + assignment.Employee.PersonalDetails.LastNameFa,assignment.AssignedAt,assignment.ReturnedAt,assignment.Notes,assignment.ReturnedAt == null,assignment.CreatedAt,assignment.ModifiedAt)).ToList(),
                item.CreatedAt,item.ModifiedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<Asset?> GetByIdAsync(long id,CancellationToken cancellationToken = default) => _dbContext.Assets.SingleOrDefaultAsync(item => item.Id == id,cancellationToken);
    public Task<bool> AssetTypeExistsAsync(short id,CancellationToken cancellationToken = default) => _dbContext.AssetTypes.AnyAsync(item => item.Id == id && item.IsActive,cancellationToken);
    public Task<bool> EmployeeExistsAsync(long employeeId,CancellationToken cancellationToken = default) => _dbContext.Employees.AnyAsync(item => item.Id == employeeId && item.IsActive,cancellationToken);

    public Task<bool> IdentifierExistsAsync(string propertyName,string value,long? excludingId = null,CancellationToken cancellationToken = default)
    {
        return propertyName switch
        {
            nameof(Asset.AssetTag) => _dbContext.Assets.AnyAsync(item => item.AssetTag == value && (!excludingId.HasValue || item.Id != excludingId.Value),cancellationToken),
            nameof(Asset.ServiceNumber) => _dbContext.Assets.AnyAsync(item => item.ServiceNumber == value && (!excludingId.HasValue || item.Id != excludingId.Value),cancellationToken),
            nameof(Asset.Imei) => _dbContext.Assets.AnyAsync(item => item.Imei == value && (!excludingId.HasValue || item.Id != excludingId.Value),cancellationToken),
            nameof(Asset.SerialNumber) => _dbContext.Assets.AnyAsync(item => item.SerialNumber == value && (!excludingId.HasValue || item.Id != excludingId.Value),cancellationToken),
            _ => Task.FromResult(false)
        };
    }

    public Task<EmployeeAssetAssignment?> GetCurrentAssignmentAsync(long assetId,CancellationToken cancellationToken = default) => 
        _dbContext.EmployeeAssetAssignments.SingleOrDefaultAsync(item => item.AssetId == assetId && item.ReturnedAt == null,cancellationToken);
    public void Add(Asset asset) => _dbContext.Assets.Add(asset);
    public void Add(EmployeeAssetAssignment assignment) => 
        _dbContext.EmployeeAssetAssignments.Add(assignment);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
