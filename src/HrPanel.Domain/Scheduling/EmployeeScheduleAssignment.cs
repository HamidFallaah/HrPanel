using HrPanel.Domain.Common;

namespace HrPanel.Domain.Scheduling;

public sealed class EmployeeScheduleAssignment: AuditableEntity<long>
{
    private EmployeeScheduleAssignment()
    {

    }

    private EmployeeScheduleAssignment(long employmentId,long workScheduleId,DateOnly effectiveFrom,short rotationOffsetDays)
    {
        ValidateCreation(employmentId,workScheduleId,effectiveFrom,rotationOffsetDays);

        EmploymentId = employmentId;
        WorkScheduleId = workScheduleId;
        EffectiveFrom = effectiveFrom;
        RotationOffsetDays = rotationOffsetDays;
    }

    public long EmploymentId { get; private set; }
    public long WorkScheduleId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public short RotationOffsetDays { get; private set; }
    public bool IsCurrent => EffectiveTo is null;
    public Employment.Employment Employment { get; private set; } = null!;
    public WorkSchedule WorkSchedule { get; private set; } = null!;

    public static EmployeeScheduleAssignment Create(long employmentId,long workScheduleId,DateOnly effectiveFrom,short rotationOffsetDays = 0)
    {
        return new EmployeeScheduleAssignment( employmentId,workScheduleId,effectiveFrom,rotationOffsetDays);
    }

    public void End(DateOnly effectiveTo)
    {
        if (EffectiveTo.HasValue)
        {
            throw new DomainRuleException("برنامه کاری قبلاً پایان یافته است");
        }

        if (effectiveTo < EffectiveFrom)
        {
            throw new DomainRuleException("تاریخ پایان برنامه کاری نمی‌تواند مقدم بر تاریخ شروع باشد");
        }

        EffectiveTo = effectiveTo;
    }

    private static void ValidateCreation(long employmentId,long workScheduleId,DateOnly effectiveFrom,short rotationOffsetDays)
    {
        if (employmentId <= 0)
        {
            throw new DomainRuleException( "شناسه استخدام باید معتبر باشد");
        }

        if (workScheduleId <= 0)
        {
            throw new DomainRuleException("شناسه برنامه کاری باید معتبر باشد");
        }

        if (effectiveFrom == default)
        {
            throw new DomainRuleException("تاریخ شروع برنامه کاری الزامی است");
        }

       if (rotationOffsetDays is < 0 or > 365)
        {
            throw new DomainRuleException("جابجایی شروع چرخه باید بین صفر تا ۳۶۵ روز باشد");
        }
    }
}