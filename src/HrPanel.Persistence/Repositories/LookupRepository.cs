using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Application.Dtos.Lookups;
using HrPanel.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace HrPanel.Persistence.Repositories;

public sealed class LookupRepository : ILookupRepository
{
    private readonly HrDbContext _dbContext;
    public LookupRepository(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetEmploymentTypesAsync(CancellationToken cancellationToken = default) => await _dbContext.EmploymentTypes.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Id).Select(x => new ReferenceLookupItemDto(x.Id,x.Code,x.NameFa,x.NameEn)).ToListAsync(cancellationToken);
    public async Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetEmploymentStatusesAsync(CancellationToken cancellationToken = default) => await _dbContext.EmploymentStatuses.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Id).Select(x => new ReferenceLookupItemDto(x.Id,x.Code,x.NameFa,x.NameEn)).ToListAsync(cancellationToken);
    public async Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetWorkTimeTypesAsync(CancellationToken cancellationToken = default) => await _dbContext.WorkTimeTypes.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Id).Select(x => new ReferenceLookupItemDto(x.Id,x.Code,x.NameFa,x.NameEn)).ToListAsync(cancellationToken);
    public async Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetOrganizationUnitTypesAsync(CancellationToken cancellationToken = default) => await _dbContext.OrganizationUnitTypes.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.HierarchyOrder).Select(x => new ReferenceLookupItemDto(x.Id,x.Code,x.NameFa,x.NameEn)).ToListAsync(cancellationToken);
    public async Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetJobLevelsAsync(CancellationToken cancellationToken = default) => await _dbContext.JobLevels.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Rank).Select(x => new ReferenceLookupItemDto(x.Id,x.Code,x.TitleFa,x.TitleEn)).ToListAsync(cancellationToken);
    public async Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetOrganizationUnitsAsync(CancellationToken cancellationToken = default) => await _dbContext.OrganizationUnits.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Code).Select(x => new ReferenceLookupItemDto(x.Id,x.Code,x.NameFa,x.NameEn)).ToListAsync(cancellationToken);
    public async Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetPositionsAsync(CancellationToken cancellationToken = default) => await _dbContext.Positions.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Code).Select(x => new ReferenceLookupItemDto(x.Id,x.Code,x.TitleFa,x.TitleEn)).ToListAsync(cancellationToken);
    public async Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetWorkLocationsAsync(CancellationToken cancellationToken = default) => await _dbContext.WorkLocations.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Code).Select(x => new ReferenceLookupItemDto(x.Id,x.Code,x.NameFa,x.NameEn)).ToListAsync(cancellationToken);
    public async Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetOperationalGroupsAsync(CancellationToken cancellationToken = default) => await _dbContext.OperationalGroups.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Code).Select(x => new ReferenceLookupItemDto(x.Id,x.Code,x.Name,null)).ToListAsync(cancellationToken); 
    public async Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetShiftsAsync(CancellationToken cancellationToken = default) => await _dbContext.Shifts.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Code).Select(x => new ReferenceLookupItemDto(x.Id,x.Code,x.NameFa,x.NameEn)).ToListAsync(cancellationToken);
    public async Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetWorkSchedulesAsync(CancellationToken cancellationToken = default) => await _dbContext.WorkSchedules.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Code).Select(x => new ReferenceLookupItemDto(x.Id,x.Code,x.NameFa,x.NameEn)).ToListAsync(cancellationToken);
    public async Task<IReadOnlyCollection<ReferenceLookupItemDto>> GetAssetTypesAsync(CancellationToken cancellationToken = default) => await _dbContext.AssetTypes.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Id).Select(x => new ReferenceLookupItemDto(x.Id,x.Code,x.NameFa,x.NameEn)).ToListAsync(cancellationToken);
}
