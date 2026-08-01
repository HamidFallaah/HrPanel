using HrPanel.Domain.Common;

namespace HrPanel.Domain.Scheduling;

public sealed class Shift : AuditableEntity<long>
{
    private Shift()
    {

    }

    private Shift(string code,string nameFa,string? nameEn,TimeOnly startTime,TimeOnly endTime,decimal workHours)
    {
        SetDetails(code,nameFa,nameEn,startTime,endTime,workHours);

        IsActive = true;
    }
    public string Code { get; private set; } = null!;
    public string NameFa { get; private set; } = null!;
    public string? NameEn { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public decimal WorkHours { get; private set; }
    public bool IsActive { get; private set; }

    public static Shift Create(string code,string nameFa,string? nameEn,TimeOnly startTime,TimeOnly endTime,decimal workHours)
    {
        return new Shift(code,nameFa,nameEn,startTime,endTime,workHours);
    }

    public void Update(string code,string nameFa,string? nameEn,TimeOnly startTime,TimeOnly endTime,decimal workHours)
    {
        SetDetails(code,nameFa,nameEn,startTime,endTime,workHours);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void SetDetails(string code,string nameFa,string? nameEn,TimeOnly startTime,TimeOnly endTime,decimal workHours)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainRuleException("کد شیفت الزامی است");
        }

        if (string.IsNullOrWhiteSpace(nameFa))
        {
            throw new DomainRuleException("نام فارسی شیفت الزامی است");
        }

        if (workHours <= 0 || workHours > 24)
        {
            throw new DomainRuleException("ساعات کاری شیفت باید بیشتر از صفر و حداکثر ۲۴ ساعت باشد");
        }

        var shiftDuration = CalculateDuration(startTime,endTime);

        if (workHours > shiftDuration)
        {
            throw new DomainRuleException("ساعات کاری نمی ‌تواند بیشتر از مدت زمانی شیفت باشد");
        }

        Code = code.Trim().ToUpperInvariant();
        NameFa = nameFa.Trim();

        NameEn = string.IsNullOrWhiteSpace(nameEn)? null: nameEn.Trim();

        StartTime = startTime;
        EndTime = endTime;
        WorkHours = workHours;
    }

    private static decimal CalculateDuration(
        TimeOnly startTime,
        TimeOnly endTime)
    {
        var start = startTime.ToTimeSpan();
        var end = endTime.ToTimeSpan();

        // Examples:
        // 08:00 -> 17:15 = 9 hours and 15 minutes
        // 22:00 -> 06:00 = 8 hours
        // 08:00 -> 08:00 = 24 hours
        var duration = end > start? end - start: TimeSpan.FromDays(1) - start + end;

        return (decimal)duration.TotalHours;
    }
}