using HrPanel.Domain.Common;
using HrPanel.Domain.Organization;

namespace HrPanel.Domain.Employment;

public sealed class EmployeeOperationalGroupAssignment: AuditableEntity<long>
{
    private EmployeeOperationalGroupAssignment()
    {

    }
    private EmployeeOperationalGroupAssignment(long employmentId,long operationalGroupId,DateOnly effectiveFrom,bool isPrimary)
    {
        if (employmentId <= 0)
        {
            throw new DomainRuleException("Employment ID must be greater than zero.");
        }

        if (operationalGroupId <= 0)
        {
            throw new DomainRuleException("Operational group ID must be greater than zero.");
        }

        EmploymentId = employmentId;
        OperationalGroupId = operationalGroupId;
        EffectiveFrom = effectiveFrom;
        IsPrimary = isPrimary;
    }

    public long EmploymentId { get; private set; }
    public long OperationalGroupId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public bool IsPrimary { get; private set; }
    public bool IsCurrent => EffectiveTo is null;
    public Employment Employment { get; private set; } = null!;
    public OperationalGroup OperationalGroup { get; private set; } = null!;
    public static EmployeeOperationalGroupAssignment Create(long employmentId,long operationalGroupId, DateOnly effectiveFrom,bool isPrimary = true)
    {
        return new EmployeeOperationalGroupAssignment(employmentId,operationalGroupId,effectiveFrom,isPrimary);
    }

    public void MakePrimary()
    {
        IsPrimary = true;
    }

    public void MakeSecondary()
    {
        IsPrimary = false;
    }
    public void End(DateOnly effectiveTo)
    {
        if (EffectiveTo.HasValue)
        {
            throw new DomainRuleException("The operational-group assignment has already ended.");
        }

        if (effectiveTo < EffectiveFrom)
        {
            throw new DomainRuleException("The end date cannot be earlier than the effective date.");
        }

        EffectiveTo = effectiveTo;
    }
}