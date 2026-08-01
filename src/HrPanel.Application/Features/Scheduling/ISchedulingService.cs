using HrPanel.Application.Common.Models;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Dtos.Scheduling;

namespace HrPanel.Application.Features.Scheduling;

public interface ISchedulingService
{
    Task<Result<PagedResult<ShiftDto>>> GetShiftsAsync(GetSchedulingItemsDto request,CancellationToken cancellationToken = default);
    Task<Result<ShiftDto>> GetShiftAsync(long id,CancellationToken cancellationToken = default);
    Task<Result<long>> CreateShiftAsync(SaveShiftDto request,CancellationToken cancellationToken = default);
    Task<Result> UpdateShiftAsync(long id,SaveShiftDto request,CancellationToken cancellationToken = default);
    Task<Result> ChangeShiftStatusAsync(long id,bool isActive,CancellationToken cancellationToken = default);

    Task<Result<PagedResult<WorkScheduleListItemDto>>> GetWorkSchedulesAsync(GetSchedulingItemsDto request,CancellationToken cancellationToken = default);
    Task<Result<WorkScheduleDetailsDto>> GetWorkScheduleAsync(long id,CancellationToken cancellationToken = default);
    Task<Result<long>> CreateWorkScheduleAsync(CreateWorkScheduleDto request,CancellationToken cancellationToken = default);
    Task<Result> UpdateWorkScheduleAsync(long id,UpdateWorkScheduleDto request,CancellationToken cancellationToken = default);
    Task<Result> SetWorkScheduleDayAsync(long id,SetWorkScheduleDayDto request,CancellationToken cancellationToken = default);
    Task<Result> RemoveWorkScheduleDayAsync(long id,short dayIndex,CancellationToken cancellationToken = default);
    Task<Result> ChangeWorkScheduleStatusAsync(long id,bool isActive,CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<ScheduleAssignmentDto>>> GetScheduleAssignmentsAsync(long employmentId,CancellationToken cancellationToken = default);
    Task<Result<long>> AssignWorkScheduleAsync(AssignWorkScheduleDto request,CancellationToken cancellationToken = default);
    Task<Result> EndScheduleAssignmentAsync(long assignmentId,EndScheduleAssignmentDto request,CancellationToken cancellationToken = default);
}
