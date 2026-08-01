using HrPanel.Domain.Common;

namespace HrPanel.Domain.Employees;

public sealed class EmployeeIdentifier : AuditableEntity<long>
{
    private EmployeeIdentifier()
    {

    }

    private EmployeeIdentifier(IdentifierType type,string value,DateOnly? effectiveFrom)
    {
        Type = type;
        SetValue(value);
        EffectiveFrom = effectiveFrom;
    }

    public long EmployeeId { get; private set; }
    public IdentifierType Type { get; private set; }
    public string Value { get; private set; } = null!;
    public DateOnly? EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public Employee Employee { get; private set; } = null!;
    public static EmployeeIdentifier Create(IdentifierType type,string value,DateOnly? effectiveFrom = null)
    {
        return new EmployeeIdentifier(type,value,effectiveFrom);
    }

    public void End(DateOnly effectiveTo)
    {
        if (EffectiveTo.HasValue)
        {
            throw new DomainRuleException("این شناسه قبلاً پایان یافته است");
        } 
        if (EffectiveFrom.HasValue && effectiveTo < EffectiveFrom.Value)
        {
            throw new DomainRuleException("تاریخ پایان شناسه نمی‌تواند قبل از تاریخ شروع آن باشد");
        }

        EffectiveTo = effectiveTo;
    }

    private void SetValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainRuleException("مقدار شناسه الزامی است");
        }

        Value = value.Trim();
    }
}