using FluentValidation;
using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Application.Common.Models;
using HrPanel.Application.Common.Results;
using HrPanel.Application.Common.Validation;
using HrPanel.Application.Dtos.Scheduling;
using HrPanel.Domain.Scheduling;

namespace HrPanel.Application.Features.Scheduling;

public sealed class SchedulingService : ISchedulingService
{
    private readonly ISchedulingRepository _repository;
    private readonly IValidator<GetSchedulingItemsDto> _queryValidator;
    private readonly IValidator<SaveShiftDto> _shiftValidator;
    private readonly IValidator<CreateWorkScheduleDto> _createScheduleValidator;
    private readonly IValidator<UpdateWorkScheduleDto> _updateScheduleValidator;
    private readonly IValidator<SetWorkScheduleDayDto> _dayValidator;
    private readonly IValidator<AssignWorkScheduleDto> _assignValidator;
    private readonly IValidator<EndScheduleAssignmentDto> _endValidator;

    public SchedulingService(
        ISchedulingRepository repository,
        IValidator<GetSchedulingItemsDto> queryValidator,
        IValidator<SaveShiftDto> shiftValidator,
        IValidator<CreateWorkScheduleDto> createScheduleValidator,
        IValidator<UpdateWorkScheduleDto> updateScheduleValidator,
        IValidator<SetWorkScheduleDayDto> dayValidator,
        IValidator<AssignWorkScheduleDto> assignValidator,
        IValidator<EndScheduleAssignmentDto> endValidator)
    {
        _repository = repository;
        _queryValidator = queryValidator;
        _shiftValidator = shiftValidator;
        _createScheduleValidator = createScheduleValidator;
        _updateScheduleValidator = updateScheduleValidator;
        _dayValidator = dayValidator;
        _assignValidator = assignValidator;
        _endValidator = endValidator;
    }

    public async Task<Result<PagedResult<ShiftDto>>> GetShiftsAsync(GetSchedulingItemsDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _queryValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<PagedResult<ShiftDto>>.Failure(validation.ToValidationError());
        return Result<PagedResult<ShiftDto>>.Success(await _repository.GetShiftsAsync(request,cancellationToken));
    }

    public async Task<Result<ShiftDto>> GetShiftAsync(long id,CancellationToken cancellationToken = default)
    {
        var shift = await _repository.GetShiftAsync(id,cancellationToken);
        return shift is null
            ? Result<ShiftDto>.Failure(SchedulingErrors.ShiftNotFound(id))
            : Result<ShiftDto>.Success(new ShiftDto(shift.Id,shift.Code,shift.NameFa,shift.NameEn,shift.StartTime,shift.EndTime,shift.WorkHours,shift.IsActive,shift.CreatedAt,shift.ModifiedAt));
    }

    public async Task<Result<long>> CreateShiftAsync(SaveShiftDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _shiftValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());
        var code = request.Code.Trim().ToUpperInvariant();
        if (await _repository.ShiftCodeExistsAsync(code,cancellationToken: cancellationToken)) return Result<long>.Failure(SchedulingErrors.CodeExists(code));
        var shift = Shift.Create(code,request.NameFa,request.NameEn,request.StartTime,request.EndTime,request.WorkHours);
        _repository.Add(shift);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(shift.Id);
    }

    public async Task<Result> UpdateShiftAsync(long id,SaveShiftDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _shiftValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var shift = await _repository.GetShiftAsync(id,cancellationToken);
        if (shift is null) return Result.Failure(SchedulingErrors.ShiftNotFound(id));
        var code = request.Code.Trim().ToUpperInvariant();
        if (await _repository.ShiftCodeExistsAsync(code,id,cancellationToken)) return Result.Failure(SchedulingErrors.CodeExists(code));
        shift.Update(code,request.NameFa,request.NameEn,request.StartTime,request.EndTime,request.WorkHours);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ChangeShiftStatusAsync(long id,bool isActive,CancellationToken cancellationToken = default)
    {
        var shift = await _repository.GetShiftAsync(id,cancellationToken);
        if (shift is null) return Result.Failure(SchedulingErrors.ShiftNotFound(id));
        if (isActive) shift.Activate(); else shift.Deactivate();
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<PagedResult<WorkScheduleListItemDto>>> GetWorkSchedulesAsync(GetSchedulingItemsDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _queryValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<PagedResult<WorkScheduleListItemDto>>.Failure(validation.ToValidationError());
        return Result<PagedResult<WorkScheduleListItemDto>>.Success(await _repository.GetWorkSchedulesAsync(request,cancellationToken));
    }

    public async Task<Result<WorkScheduleDetailsDto>> GetWorkScheduleAsync(long id,CancellationToken cancellationToken = default)
    {
        var schedule = await _repository.GetWorkScheduleDetailsAsync(id,cancellationToken);
        return schedule is null
            ? Result<WorkScheduleDetailsDto>.Failure(SchedulingErrors.WorkScheduleNotFound(id))
            : Result<WorkScheduleDetailsDto>.Success(schedule);
    }

    public async Task<Result<long>> CreateWorkScheduleAsync(CreateWorkScheduleDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _createScheduleValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());
        var code = request.Code.Trim().ToUpperInvariant();
        if (await _repository.WorkScheduleCodeExistsAsync(code,cancellationToken: cancellationToken)) return Result<long>.Failure(SchedulingErrors.CodeExists(code));

        var shiftIds = request.Days.Where(day => !day.IsRestDay).Select(day => day.ShiftId!.Value).Distinct().ToArray();
        if (!await _repository.ShiftsExistAsync(shiftIds,cancellationToken)) return Result<long>.Failure(SchedulingErrors.InvalidShift());

        var schedule = WorkSchedule.Create(code,request.NameFa,request.NameEn,request.PatternType,request.CycleLengthDays,request.AnchorDate);
        foreach (var day in request.Days)
        {
            if (day.IsRestDay) schedule.AddRestDay(day.DayIndex);
            else schedule.AddWorkingDay(day.DayIndex,day.ShiftId!.Value);
        }

        _repository.Add(schedule);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(schedule.Id);
    }

    public async Task<Result> UpdateWorkScheduleAsync(long id,UpdateWorkScheduleDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _updateScheduleValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var schedule = await _repository.GetWorkScheduleAsync(id,cancellationToken);
        if (schedule is null) return Result.Failure(SchedulingErrors.WorkScheduleNotFound(id));
        if (schedule.Days.Any(day => day.DayIndex >= request.CycleLengthDays)) return Result.Failure(SchedulingErrors.InvalidCycle());
        var code = request.Code.Trim().ToUpperInvariant();
        if (await _repository.WorkScheduleCodeExistsAsync(code,id,cancellationToken)) return Result.Failure(SchedulingErrors.CodeExists(code));
        schedule.Update(code,request.NameFa,request.NameEn,request.PatternType,request.CycleLengthDays,request.AnchorDate);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> SetWorkScheduleDayAsync(long id,SetWorkScheduleDayDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _dayValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var schedule = await _repository.GetWorkScheduleAsync(id,cancellationToken);
        if (schedule is null) return Result.Failure(SchedulingErrors.WorkScheduleNotFound(id));
        if (request.DayIndex >= schedule.CycleLengthDays) return Result.Failure(SchedulingErrors.InvalidCycle());
        if (!request.IsRestDay && !await _repository.ShiftsExistAsync([request.ShiftId!.Value],cancellationToken)) return Result.Failure(SchedulingErrors.InvalidShift());
        if (request.IsRestDay) schedule.SetRestDay(request.DayIndex);
        else schedule.SetWorkingDay(request.DayIndex,request.ShiftId!.Value);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RemoveWorkScheduleDayAsync(long id,short dayIndex,CancellationToken cancellationToken = default)
    {
        var schedule = await _repository.GetWorkScheduleAsync(id,cancellationToken);
        if (schedule is null) return Result.Failure(SchedulingErrors.WorkScheduleNotFound(id));
        if (schedule.Days.All(day => day.DayIndex != dayIndex)) return Result.Failure(Error.NotFound("Scheduling.DayNotFound",$"روز {dayIndex} در برنامه کاری یافت نشد"));
        schedule.RemoveDay(dayIndex);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ChangeWorkScheduleStatusAsync(long id,bool isActive,CancellationToken cancellationToken = default)
    {
        var schedule = await _repository.GetWorkScheduleAsync(id,cancellationToken);
        if (schedule is null) return Result.Failure(SchedulingErrors.WorkScheduleNotFound(id));
        if (isActive) schedule.Activate(); else schedule.Deactivate();
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<ScheduleAssignmentDto>>> GetScheduleAssignmentsAsync(long employmentId,CancellationToken cancellationToken = default)
    {
        if (!await _repository.EmploymentExistsAsync(employmentId,cancellationToken)) return Result<IReadOnlyCollection<ScheduleAssignmentDto>>.Failure(SchedulingErrors.EmploymentNotFound(employmentId));
        var assignments = await _repository.GetScheduleAssignmentsAsync(employmentId,cancellationToken);
        return Result<IReadOnlyCollection<ScheduleAssignmentDto>>.Success(assignments);
    }

    public async Task<Result<long>> AssignWorkScheduleAsync(AssignWorkScheduleDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _assignValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result<long>.Failure(validation.ToValidationError());
        if (!await _repository.CurrentEmploymentExistsAsync(request.EmploymentId,cancellationToken)) return Result<long>.Failure(SchedulingErrors.EmploymentNotFound(request.EmploymentId));
        var schedule = await _repository.GetWorkScheduleAsync(request.WorkScheduleId,cancellationToken);
        if (schedule is null || !schedule.IsActive) return Result<long>.Failure(SchedulingErrors.WorkScheduleNotFound(request.WorkScheduleId));
        if (await _repository.CurrentScheduleAssignmentExistsAsync(request.EmploymentId,cancellationToken)) return Result<long>.Failure(SchedulingErrors.CurrentAssignmentExists());
        var assignment = EmployeeScheduleAssignment.Create(request.EmploymentId,request.WorkScheduleId,request.EffectiveFrom,request.RotationOffsetDays);
        _repository.Add(assignment);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<long>.Success(assignment.Id);
    }

    public async Task<Result> EndScheduleAssignmentAsync(long assignmentId,EndScheduleAssignmentDto request,CancellationToken cancellationToken = default)
    {
        var validation = await _endValidator.ValidateAsync(request,cancellationToken);
        if (!validation.IsValid) return Result.Failure(validation.ToValidationError());
        var assignment = await _repository.GetScheduleAssignmentAsync(assignmentId,cancellationToken);
        if (assignment is null) return Result.Failure(SchedulingErrors.ScheduleAssignmentNotFound(assignmentId));
        if (assignment.EffectiveTo.HasValue) return Result.Failure(Error.Conflict("Scheduling.AssignmentEnded","این تخصیص قبلاً پایان یافته است"));
        if (request.EffectiveTo < assignment.EffectiveFrom) return Result.Failure(Error.Failure("Scheduling.InvalidEndDate","تاریخ پایان نمی‌تواند قبل از تاریخ شروع باشد"));
        assignment.End(request.EffectiveTo);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
