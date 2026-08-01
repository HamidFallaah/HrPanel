using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Application.Common.Models;
using HrPanel.Application.Dtos.Scheduling;
using HrPanel.Domain.Scheduling;
using HrPanel.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace HrPanel.Persistence.Repositories;

public sealed class SchedulingRepository : ISchedulingRepository
{
    private readonly HrDbContext _dbContext;

    public SchedulingRepository(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<PagedResult<ShiftDto>> GetShiftsAsync(GetSchedulingItemsDto request,CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Shifts.AsNoTracking();
        if (request.IsActive.HasValue) query = query.Where(item => item.IsActive == request.IsActive.Value);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(item => item.Code.Contains(search) || item.NameFa.Contains(search) || (item.NameEn != null && item.NameEn.Contains(search)));
        }
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(item => item.Code).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(item => new ShiftDto(item.Id,item.Code,item.NameFa,item.NameEn,item.StartTime,item.EndTime,item.WorkHours,item.IsActive,item.CreatedAt,item.ModifiedAt)).ToListAsync(cancellationToken);
        return PagedResult<ShiftDto>.Create(items,request.PageNumber,request.PageSize,totalCount);
    }

    public Task<Shift?> GetShiftAsync(long id,CancellationToken cancellationToken = default) => _dbContext.Shifts.SingleOrDefaultAsync(item => item.Id == id,cancellationToken);
    public Task<bool> ShiftCodeExistsAsync(string code,long? excludingId = null,CancellationToken cancellationToken = default) => _dbContext.Shifts.AnyAsync(item => item.Code == code && (!excludingId.HasValue || item.Id != excludingId.Value),cancellationToken);

    public async Task<PagedResult<WorkScheduleListItemDto>> GetWorkSchedulesAsync(GetSchedulingItemsDto request,CancellationToken cancellationToken = default)
    {
        var query = _dbContext.WorkSchedules.AsNoTracking();
        if (request.IsActive.HasValue) query = query.Where(item => item.IsActive == request.IsActive.Value);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(item => item.Code.Contains(search) || item.NameFa.Contains(search) || (item.NameEn != null && item.NameEn.Contains(search)));
        }
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(item => item.Code).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .Select(item => new WorkScheduleListItemDto(item.Id,item.Code,item.NameFa,item.NameEn,item.PatternType,item.CycleLengthDays,item.AnchorDate,item.IsActive)).ToListAsync(cancellationToken);
        return PagedResult<WorkScheduleListItemDto>.Create(items,request.PageNumber,request.PageSize,totalCount);
    }

    public Task<WorkScheduleDetailsDto?> GetWorkScheduleDetailsAsync(long id,CancellationToken cancellationToken = default)
    {
        return _dbContext.WorkSchedules.AsNoTracking().Where(item => item.Id == id)
            .Select(item => new WorkScheduleDetailsDto(
                item.Id,item.Code,item.NameFa,item.NameEn,item.PatternType,item.CycleLengthDays,item.AnchorDate,item.IsActive,
                item.Days.OrderBy(day => day.DayIndex).Select(day => new WorkScheduleDayDto(day.Id,day.DayIndex,day.ShiftId,day.Shift == null ? null : day.Shift.NameFa,day.IsRestDay)).ToList(),
                item.CreatedAt,item.ModifiedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<WorkSchedule?> GetWorkScheduleAsync(long id,CancellationToken cancellationToken = default) => _dbContext.WorkSchedules.Include(item => item.Days).SingleOrDefaultAsync(item => item.Id == id,cancellationToken);
    public Task<bool> WorkScheduleCodeExistsAsync(string code,long? excludingId = null,CancellationToken cancellationToken = default) => _dbContext.WorkSchedules.AnyAsync(item => item.Code == code && (!excludingId.HasValue || item.Id != excludingId.Value),cancellationToken);

    public async Task<bool> ShiftsExistAsync(IEnumerable<long> shiftIds,CancellationToken cancellationToken = default)
    {
        var ids = shiftIds.Distinct().ToArray();
        if (ids.Length == 0) return true;
        var count = await _dbContext.Shifts.CountAsync(item => ids.Contains(item.Id) && item.IsActive,cancellationToken);
        return count == ids.Length;
    }

    public Task<bool> EmploymentExistsAsync(long employmentId,CancellationToken cancellationToken = default) => _dbContext.Employments.AnyAsync(item => item.Id == employmentId,cancellationToken);
    public Task<bool> CurrentEmploymentExistsAsync(long employmentId,CancellationToken cancellationToken = default) => _dbContext.Employments.AnyAsync(item => item.Id == employmentId && item.EndDate == null,cancellationToken);
    public Task<bool> CurrentScheduleAssignmentExistsAsync(long employmentId,CancellationToken cancellationToken = default) => _dbContext.EmployeeScheduleAssignments.AnyAsync(item => item.EmploymentId == employmentId && item.EffectiveTo == null,cancellationToken);
    public Task<EmployeeScheduleAssignment?> GetScheduleAssignmentAsync(long assignmentId,CancellationToken cancellationToken = default) => _dbContext.EmployeeScheduleAssignments.SingleOrDefaultAsync(item => item.Id == assignmentId,cancellationToken);

    public async Task<IReadOnlyCollection<ScheduleAssignmentDto>> GetScheduleAssignmentsAsync(long employmentId,CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmployeeScheduleAssignments.AsNoTracking().Where(item => item.EmploymentId == employmentId)
            .OrderByDescending(item => item.EffectiveFrom)
            .Select(item => new ScheduleAssignmentDto(
                item.Id,item.EmploymentId,item.Employment.EmployeeId,item.Employment.Employee.EmployeeNumber,
                item.Employment.Employee.PersonalDetails == null ? item.Employment.Employee.EmployeeNumber : item.Employment.Employee.PersonalDetails.FirstNameFa + " " + item.Employment.Employee.PersonalDetails.LastNameFa,
                item.WorkScheduleId,item.WorkSchedule.NameFa,item.EffectiveFrom,item.EffectiveTo,item.RotationOffsetDays,item.EffectiveTo == null,item.CreatedAt,item.ModifiedAt))
            .ToListAsync(cancellationToken);
    }
    public void Add(Shift shift) => _dbContext.Shifts.Add(shift);
    public void Add(WorkSchedule schedule) => _dbContext.WorkSchedules.Add(schedule);
    public void Add(EmployeeScheduleAssignment assignment) => _dbContext.EmployeeScheduleAssignments.Add(assignment);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _dbContext.SaveChangesAsync(cancellationToken);
}
