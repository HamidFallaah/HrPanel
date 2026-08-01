using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Application.Common.Models;
using HrPanel.Application.Dtos.Organization;
using HrPanel.Domain.Organization;
using HrPanel.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace HrPanel.Persistence.Repositories;

public sealed class OrganizationRepository : IOrganizationRepository
{
    private readonly HrDbContext _dbContext;

    public OrganizationRepository(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<OrganizationUnitDto>> GetOrganizationUnitsAsync(GetOrganizationUnitsDto request,CancellationToken cancellationToken = default)
    {
        var query = _dbContext.OrganizationUnits.AsNoTracking();
        if (request.IsActive.HasValue) query = query.Where(item => item.IsActive == request.IsActive.Value);
        if (request.OrganizationUnitTypeId.HasValue) query = query.Where(item => item.OrganizationUnitTypeId == request.OrganizationUnitTypeId.Value);
        if (request.ParentOrganizationUnitId.HasValue) query = query.Where(item => item.ParentOrganizationUnitId == request.ParentOrganizationUnitId.Value);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(item => item.Code.Contains(search) || item.NameFa.Contains(search) || (item.NameEn != null && item.NameEn.Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(item => item.Code)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(item => new OrganizationUnitDto(item.Id,item.OrganizationUnitTypeId,item.OrganizationUnitType.NameFa,item.ParentOrganizationUnitId,item.ParentOrganizationUnit == null ? null : item.ParentOrganizationUnit.NameFa,item.Code,item.NameFa,item.NameEn,item.IsActive,item.CreatedAt,item.ModifiedAt))
            .ToListAsync(cancellationToken);
        return PagedResult<OrganizationUnitDto>.Create(items,request.PageNumber,request.PageSize,totalCount);
    }

    public async Task<IReadOnlyCollection<OrganizationUnitTreeDto>> GetOrganizationTreeAsync(bool includeInactive,CancellationToken cancellationToken = default)
    {
        var query = _dbContext.OrganizationUnits.AsNoTracking();
        if (!includeInactive) query = query.Where(item => item.IsActive);
        var units = await query.OrderBy(item => item.Code)
            .Select(item => new FlatUnit(item.Id,item.ParentOrganizationUnitId,item.Code,item.NameFa,item.NameEn,item.OrganizationUnitTypeId,item.OrganizationUnitType.NameFa,item.IsActive))
            .ToListAsync(cancellationToken);
        return BuildTree(units,null);
    }

    public Task<OrganizationUnit?> GetOrganizationUnitAsync(long id,CancellationToken cancellationToken = default)
    {
        return _dbContext.OrganizationUnits.Include(item => item.OrganizationUnitType).Include(item => item.ParentOrganizationUnit).SingleOrDefaultAsync(item => item.Id == id,cancellationToken);
    }

    public Task<bool> OrganizationUnitTypeExistsAsync(short id,CancellationToken cancellationToken = default) => _dbContext.OrganizationUnitTypes.AnyAsync(item => item.Id == id && item.IsActive,cancellationToken);

    public Task<bool> OrganizationUnitCodeExistsAsync(string code,long? excludingId = null,CancellationToken cancellationToken = default) => _dbContext.OrganizationUnits.AnyAsync(item => item.Code == code && (!excludingId.HasValue || item.Id != excludingId.Value),cancellationToken);

    public async Task<bool> WouldCreateOrganizationCycleAsync(long unitId,long parentId,CancellationToken cancellationToken = default)
    {
        var currentId = (long?)parentId;
        var visited = new HashSet<long>();

        while (currentId.HasValue)
        {
            if (currentId.Value == unitId) return true;
            if (!visited.Add(currentId.Value)) return true;
            currentId = await _dbContext.OrganizationUnits.AsNoTracking().Where(item => item.Id == currentId.Value).Select(item => item.ParentOrganizationUnitId).SingleOrDefaultAsync(cancellationToken);
        }
        return false;
    }

    public async Task<PagedResult<PositionDto>> GetPositionsAsync(GetOrganizationItemsDto request,CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Positions.AsNoTracking();
        if (request.IsActive.HasValue) query = query.Where(item => item.IsActive == request.IsActive.Value);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(item => item.Code.Contains(search) || item.TitleFa.Contains(search) || (item.TitleEn != null && item.TitleEn.Contains(search)));
        }
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(item => item.Code).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(item => new PositionDto(item.Id,item.Code,item.TitleFa,item.TitleEn,item.IsActive,item.CreatedAt,item.ModifiedAt)).ToListAsync(cancellationToken);
        return PagedResult<PositionDto>.Create(items,request.PageNumber,request.PageSize,totalCount);
    }

    public Task<Position?> GetPositionAsync(long id,CancellationToken cancellationToken = default) => _dbContext.Positions.SingleOrDefaultAsync(item => item.Id == id,cancellationToken);
    public Task<bool> PositionCodeExistsAsync(string code,long? excludingId = null,CancellationToken cancellationToken = default) => _dbContext.Positions.AnyAsync(item => item.Code == code && (!excludingId.HasValue || item.Id != excludingId.Value),cancellationToken);

    public async Task<PagedResult<WorkLocationDto>> GetWorkLocationsAsync(GetOrganizationItemsDto request,CancellationToken cancellationToken = default)
    {
        var query = _dbContext.WorkLocations.AsNoTracking();
        if (request.IsActive.HasValue) query = query.Where(item => item.IsActive == request.IsActive.Value);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(item => item.Code.Contains(search) || item.NameFa.Contains(search) || (item.NameEn != null && item.NameEn.Contains(search)) || (item.City != null && item.City.Contains(search)));
        }
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(item => item.Code).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(item => new WorkLocationDto(item.Id,item.Code,item.NameFa,item.NameEn,item.Province,item.City,item.Address,item.IsActive,item.CreatedAt,item.ModifiedAt)).ToListAsync(cancellationToken);
        return PagedResult<WorkLocationDto>.Create(items,request.PageNumber,request.PageSize,totalCount);
    }

    public Task<WorkLocation?> GetWorkLocationAsync(long id,CancellationToken cancellationToken = default) => _dbContext.WorkLocations.SingleOrDefaultAsync(item => item.Id == id,cancellationToken);
    public Task<bool> WorkLocationCodeExistsAsync(string code,long? excludingId = null,CancellationToken cancellationToken = default) => _dbContext.WorkLocations.AnyAsync(item => item.Code == code && (!excludingId.HasValue || item.Id != excludingId.Value),cancellationToken);

    public async Task<PagedResult<OperationalGroupDto>> GetOperationalGroupsAsync(GetOrganizationItemsDto request,CancellationToken cancellationToken = default)
    {
        var query = _dbContext.OperationalGroups.AsNoTracking();
        if (request.IsActive.HasValue) query = query.Where(item => item.IsActive == request.IsActive.Value);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(item => item.Code.Contains(search) || item.Name.Contains(search));
        }
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(item => item.Code).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(item => new OperationalGroupDto(item.Id,item.Code,item.Name,item.Type,item.IsActive,item.CreatedAt,item.ModifiedAt)).ToListAsync(cancellationToken);
        return PagedResult<OperationalGroupDto>.Create(items,request.PageNumber,request.PageSize,totalCount);
    }

    public Task<OperationalGroup?> GetOperationalGroupAsync(long id,CancellationToken cancellationToken = default) => _dbContext.OperationalGroups.SingleOrDefaultAsync(item => item.Id == id,cancellationToken);
    public Task<bool> OperationalGroupCodeExistsAsync(string code,CancellationToken cancellationToken = default) => _dbContext.OperationalGroups.AnyAsync(item => item.Code == code,cancellationToken);

    public void Add(OrganizationUnit unit) => _dbContext.OrganizationUnits.Add(unit);
    public void Add(Position position) => _dbContext.Positions.Add(position);
    public void Add(WorkLocation location) => _dbContext.WorkLocations.Add(location);
    public void Add(OperationalGroup group) => _dbContext.OperationalGroups.Add(group);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _dbContext.SaveChangesAsync(cancellationToken);

    private static IReadOnlyCollection<OrganizationUnitTreeDto> BuildTree(IReadOnlyCollection<FlatUnit> units,long? parentId)
    {
        return units.Where(item => item.ParentId == parentId)
            .Select(item => new OrganizationUnitTreeDto(item.Id,item.Code,item.NameFa,item.NameEn,item.TypeId,item.TypeName,item.IsActive,BuildTree(units,item.Id))).ToArray();
    }
    private sealed record FlatUnit(long Id,long? ParentId,string Code,string NameFa,string? NameEn,short TypeId,string TypeName,bool IsActive);
}
