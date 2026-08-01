using HrPanel.Domain.Common;

namespace HrPanel.Domain.Scheduling;

public sealed class WorkScheduleDay : AuditableEntity<long>
{
    private WorkScheduleDay()
    {

    }
    private WorkScheduleDay(short dayIndex,long? shiftId, bool isRestDay)
    {
        if (dayIndex < 0)
        {
            throw new DomainRuleException("شاخص روز برنامه کاری نمی‌تواند منفی باشد");
        }

        if (isRestDay && shiftId.HasValue)
        {
            throw new DomainRuleException("روز استراحت نمی‌ تواند شیفت داشته باشد");
        }

        if (!isRestDay && !shiftId.HasValue)
        {
            throw new DomainRuleException("روز کاری باید شیفت داشته باشد");
        }

        if (shiftId.HasValue && shiftId.Value <= 0)
        {
            throw new DomainRuleException("شناسه شیفت باید معتبر باشد");
        }

        DayIndex = dayIndex;
        ShiftId = shiftId;
        IsRestDay = isRestDay;
    }

    public long WorkScheduleId { get; private set; }
    public short DayIndex { get; private set; }
    public long? ShiftId { get; private set; }
    public bool IsRestDay { get; private set; }
    public WorkSchedule WorkSchedule { get; private set; } = null!;
    public Shift? Shift { get; private set; }

    public static WorkScheduleDay Working(short dayIndex,long shiftId)
    {
        return new WorkScheduleDay(dayIndex,shiftId,isRestDay: false);
    }

    public static WorkScheduleDay Rest(short dayIndex)
    {
        return new WorkScheduleDay(dayIndex,shiftId: null,isRestDay: true);
    }
}