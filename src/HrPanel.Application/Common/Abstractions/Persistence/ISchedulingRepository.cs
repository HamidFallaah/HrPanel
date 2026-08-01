using HrPanel.Application.Common.Models;
using HrPanel.Application.Dtos.Scheduling;
using HrPanel.Domain.Scheduling;

namespace HrPanel.Application.Common.Abstractions.Persistence;

public interface ISchedulingRepository
{
    Task<PagedResult<ShiftDto>> GetShiftsAsync(GetSchedulingItemsDto request,CancellationToken cancellationToken = default);
    Task<Shift?> GetShiftAsync(long id,CancellationToken cancellationToken = default);
    Task<bool> ShiftCodeExistsAsync(string code,long? excludingId = null,CancellationToken cancellationToken = default);

    Task<PagedResult<WorkScheduleListItemDto>> GetWorkSchedulesAsync(GetSchedulingItemsDto request,CancellationToken cancellationToken = default);
    Task<WorkScheduleDetailsDto?> GetWorkScheduleDetailsAsync(long id,CancellationToken cancellationToken = default);
    Task<WorkSchedule?> GetWorkScheduleAsync(long id,CancellationToken cancellationToken = default);
    Task<bool> WorkScheduleCodeExistsAsync(string code,long? excludingId = null,CancellationToken cancellationToken = default);
    Task<bool> ShiftsExistAsync(IEnumerable<long> shiftIds,CancellationToken cancellationToken = default);

    Task<bool> EmploymentExistsAsync(long employmentId,CancellationToken cancellationToken = default);
    Task<bool> CurrentEmploymentExistsAsync(long employmentId,CancellationToken cancellationToken = default);
    Task<bool> CurrentScheduleAssignmentExistsAsync(long employmentId,CancellationToken cancellationToken = default);
    Task<EmployeeScheduleAssignment?> GetScheduleAssignmentAsync(long assignmentId,CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ScheduleAssignmentDto>> GetScheduleAssignmentsAsync(long employmentId,CancellationToken cancellationToken = default);

    void Add(Shift shift);
    void Add(WorkSchedule schedule);
    void Add(EmployeeScheduleAssignment assignment);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
