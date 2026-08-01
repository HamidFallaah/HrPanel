using HrPanel.Domain.Common;

namespace HrPanel.Domain.Scheduling;

public sealed class WorkSchedule : AuditableEntity<long>
{
    private readonly List<WorkScheduleDay> _days = [];
    private WorkSchedule()
    {

    }

    private WorkSchedule(string code,string nameFa,string? nameEn,WorkSchedulePatternType patternType,short cycleLengthDays,DateOnly? anchorDate)
    {
        SetDetails(code,nameFa,nameEn,patternType,cycleLengthDays,anchorDate);

        IsActive = true;
    }

    public string Code { get; private set; } = null!;
    public string NameFa { get; private set; } = null!;
    public string? NameEn { get; private set; }
    public WorkSchedulePatternType PatternType { get; private set; }
    public short CycleLengthDays { get; private set; }
    public DateOnly? AnchorDate { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<WorkScheduleDay> Days => _days.AsReadOnly();

    public static WorkSchedule Create(string code,string nameFa,string? nameEn,WorkSchedulePatternType patternType,short cycleLengthDays,DateOnly? anchorDate = null)
    {
        return new WorkSchedule(code,nameFa,nameEn,patternType,cycleLengthDays,anchorDate);
    }

    public void Update(string code,string nameFa,string? nameEn,WorkSchedulePatternType patternType,short cycleLengthDays,DateOnly? anchorDate = null)
    {
        SetDetails(code,nameFa,nameEn,patternType,cycleLengthDays,anchorDate);
    }

    public void AddWorkingDay(short dayIndex,long shiftId)
    {
        AddDay(WorkScheduleDay.Working(dayIndex,shiftId));
    }

    public void AddRestDay(short dayIndex)
    {
        AddDay(WorkScheduleDay.Rest(dayIndex));
    }

    public void SetWorkingDay(short dayIndex,long shiftId)
    {
        ReplaceDay(WorkScheduleDay.Working(dayIndex,shiftId));
    }

    public void SetRestDay(short dayIndex)
    {
        ReplaceDay(WorkScheduleDay.Rest(dayIndex));
    }

    public void RemoveDay(short dayIndex)
    {
        var day = _days.SingleOrDefault(item => item.DayIndex == dayIndex);

        if (day is null)
        {
            throw new DomainRuleException("روز برنامه کاری یافت نشد");
        }

        _days.Remove(day);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void AddDay(WorkScheduleDay day)
    {
        ArgumentNullException.ThrowIfNull(day);

        if (day.DayIndex >= CycleLengthDays)
        {
            throw new DomainRuleException("شاخص روز باید از طول چرخه برنامه کاری کمتر باشد");
        }

        if (_days.Any(existingDay =>
                existingDay.DayIndex == day.DayIndex))
        {
            throw new DomainRuleException("برای این روز از چرخه قبلاً الگو ثبت شده است");
        }

        _days.Add(day);
    }

    private void ReplaceDay(WorkScheduleDay day)
    {
        ArgumentNullException.ThrowIfNull(day);

        if (day.DayIndex >= CycleLengthDays)
        {
            throw new DomainRuleException(
                "شاخص روز باید از طول چرخه برنامه کاری کمتر باشد");
        }

        var currentDay = _days.SingleOrDefault(
            item => item.DayIndex == day.DayIndex);

        if (currentDay is not null)
        {
            _days.Remove(currentDay);
        }

        _days.Add(day);
    }

    private void SetDetails(string code,string nameFa,string? nameEn,WorkSchedulePatternType patternType,short cycleLengthDays,DateOnly? anchorDate)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainRuleException("کد برنامه کاری الزامی است");
        }

        if (string.IsNullOrWhiteSpace(nameFa))
        {
            throw new DomainRuleException("نام فارسی برنامه کاری الزامی است");
        }

        if (!Enum.IsDefined(patternType))
        {
            throw new DomainRuleException("نوع الگوی برنامه کاری معتبر نیست");
        }

        if (cycleLengthDays is <= 0 or > 366)
        {
            throw new DomainRuleException( "طول چرخه برنامه کاری باید بین ۱ تا ۳۶۶ روز باشد");
        }

        if (patternType == WorkSchedulePatternType.Weekly &&cycleLengthDays != 7)
        {
            throw new DomainRuleException("طول چرخه برنامه هفتگی باید دقیقاً ۷ روز باشد");
        }

        if (patternType == WorkSchedulePatternType.Rotating && !anchorDate.HasValue)
        {
            throw new DomainRuleException("تاریخ مبنای برنامه چرخشی الزامی است");
        }

        if (_days.Any(day => day.DayIndex >= cycleLengthDays))
        {
            throw new DomainRuleException("طول چرخه جدید با روزهای ثبت ‌شده سازگار نیست");
        }

        Code = code.Trim().ToUpperInvariant();
        NameFa = nameFa.Trim();

        NameEn = string.IsNullOrWhiteSpace(nameEn)? null: nameEn.Trim();

        PatternType = patternType;
        CycleLengthDays = cycleLengthDays;
        AnchorDate = anchorDate;
    }
}
