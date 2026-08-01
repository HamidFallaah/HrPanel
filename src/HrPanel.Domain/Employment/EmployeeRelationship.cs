using HrPanel.Domain.Common;

namespace HrPanel.Domain.Employment;

public sealed class EmployeeRelationship : AuditableEntity<long>
{
    private EmployeeRelationship()
    {

    }

    private EmployeeRelationship(long employeeId,RelationshipType type,RelationshipContext context,long? relatedEmployeeId,long? relatedExternalPersonId,DateOnly effectiveFrom)
    {
        var hasEmployee = relatedEmployeeId.HasValue;
        var hasExternalPerson = relatedExternalPersonId.HasValue;

        if (hasEmployee == hasExternalPerson)
        {
            throw new DomainRuleException("رابطه باید دقیقاً به یک کارمند یا یک شخص خارجی مرتبط باشد");
        }

        if (relatedEmployeeId == employeeId)
        {
            throw new DomainRuleException("کارمند نمی‌تواند مدیر خودش باشد");
        }

        EmployeeId = employeeId;
        Type = type;
        Context = context;
        RelatedEmployeeId = relatedEmployeeId;
        RelatedExternalPersonId = relatedExternalPersonId;
        EffectiveFrom = effectiveFrom;
    }

    public long EmployeeId { get; private set; }
    public RelationshipType Type { get; private set; }
    public RelationshipContext Context { get; private set; }
    public long? RelatedEmployeeId { get; private set; }
    public long? RelatedExternalPersonId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public bool IsCurrent => EffectiveTo is null;
    public Employees.Employee Employee { get; private set; } = null!;
    public Employees.Employee? RelatedEmployee { get; private set; }
    public ExternalPerson? RelatedExternalPerson { get; private set; }
    public static EmployeeRelationship ForEmployee(long employeeId,RelationshipType type,RelationshipContext context,long relatedEmployeeId,DateOnly effectiveFrom)
    {
        return new EmployeeRelationship(employeeId,type,context,relatedEmployeeId,null,effectiveFrom);
    }

    public static EmployeeRelationship ForExternalPerson(long employeeId,RelationshipType type,RelationshipContext context,long relatedExternalPersonId,DateOnly effectiveFrom)
    {
        return new EmployeeRelationship(employeeId,type,context,null,relatedExternalPersonId,effectiveFrom);
    }

    public void End(DateOnly effectiveTo)
    {
        if (EffectiveTo.HasValue)
        {
            throw new DomainRuleException("رابطه قبلاً پایان یافته است");
        }

        if (effectiveTo < EffectiveFrom)
        {
            throw new DomainRuleException("تاریخ پایان همکاری نمی‌تواند مقدم بر تاریخ شروع باشد");
        }

        EffectiveTo = effectiveTo;
    }
}
