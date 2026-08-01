using HrPanel.Domain.Scheduling;

namespace HrPanel.Application.Dtos.Scheduling;

public sealed record GetSchedulingItemsDto(string? Search = null,bool? IsActive = null,int PageNumber = 1,int PageSize = 20);

public sealed class SaveShiftDto
{
    public string Code { get; set; } = string.Empty;
    public string NameFa { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public decimal WorkHours { get; set; }
}
public sealed class CreateWorkScheduleDto
{
    public string Code { get; set; } = string.Empty;
    public string NameFa { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public WorkSchedulePatternType PatternType { get; set; }
    public short CycleLengthDays { get; set; }
    public DateOnly? AnchorDate { get; set; }
    public IReadOnlyCollection<SetWorkScheduleDayDto> Days { get; set; } = [];
}
public sealed class UpdateWorkScheduleDto
{
    public string Code { get; set; } = string.Empty;
    public string NameFa { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public WorkSchedulePatternType PatternType { get; set; }
    public short CycleLengthDays { get; set; }
    public DateOnly? AnchorDate { get; set; }
}
public sealed class SetWorkScheduleDayDto
{
    public short DayIndex { get; set; }
    public long? ShiftId { get; set; }
    public bool IsRestDay { get; set; }
}
public sealed class AssignWorkScheduleDto
{
    public long EmploymentId { get; set; }
    public long WorkScheduleId { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public short RotationOffsetDays { get; set; }
}
public sealed class EndScheduleAssignmentDto
{
    public DateOnly EffectiveTo { get; set; }
}
public sealed record ShiftDto(
    long Id,
    string Code,
    string NameFa,
    string? NameEn,
    TimeOnly StartTime,
    TimeOnly EndTime,
    decimal WorkHours,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? ModifiedAt);

public sealed record WorkScheduleListItemDto(
    long Id,
    string Code,
    string NameFa,
    string? NameEn,
    WorkSchedulePatternType PatternType,
    short CycleLengthDays,
    DateOnly? AnchorDate,
    bool IsActive);

public sealed record WorkScheduleDetailsDto(
    long Id,
    string Code,
    string NameFa,
    string? NameEn,
    WorkSchedulePatternType PatternType,
    short CycleLengthDays,
    DateOnly? AnchorDate,
    bool IsActive,
    IReadOnlyCollection<WorkScheduleDayDto> Days,
    DateTime CreatedAt,
    DateTime? ModifiedAt);

public sealed record WorkScheduleDayDto(long Id,short DayIndex,long? ShiftId,string? ShiftName,bool IsRestDay);

public sealed record ScheduleAssignmentDto(
    long Id,
    long EmploymentId,
    long EmployeeId,
    string EmployeeNumber,
    string EmployeeDisplayName,
    long WorkScheduleId,
    string WorkScheduleName,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    short RotationOffsetDays,
    bool IsCurrent,
    DateTime CreatedAt,
    DateTime? ModifiedAt);
