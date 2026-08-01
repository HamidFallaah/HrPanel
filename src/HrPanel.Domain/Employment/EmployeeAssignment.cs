using HrPanel.Domain.Common;

namespace HrPanel.Domain.Employment;

public sealed class EmployeeAssignment : AuditableEntity<long>
{
    private EmployeeAssignment()
    {

    }

    private EmployeeAssignment(AssignmentContext context,DateOnly effectiveFrom,long? organizationUnitId,long? positionId,short? jobLevelId,long? workLocationId)
    {
        Context = context;
        EffectiveFrom = effectiveFrom;
        OrganizationUnitId = organizationUnitId;
        PositionId = positionId;
        JobLevelId = jobLevelId;
        WorkLocationId = workLocationId;
    }

    public long EmploymentId { get; private set; }
    public AssignmentContext Context { get; private set; }
    public long? OrganizationUnitId { get; private set; }
    public long? PositionId { get; private set; }
    public short? JobLevelId { get; private set; }
    public long? WorkLocationId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public bool IsCurrent => EffectiveTo is null;
    public Employment Employment { get; private set; } = null!;
    public Organization.OrganizationUnit? OrganizationUnit { get; private set; }
    public Organization.Position? Position { get; private set; }
    public Organization.JobLevel? JobLevel { get; private set; }
    public Organization.WorkLocation? WorkLocation { get; private set; }
    public static EmployeeAssignment Create(AssignmentContext context,DateOnly effectiveFrom,long? organizationUnitId,long? positionId,short? jobLevelId,long? workLocationId)
    {
        return new EmployeeAssignment(context,effectiveFrom,organizationUnitId,positionId,jobLevelId,workLocationId);
    }

    public void End(DateOnly effectiveTo)
    {
        if (EffectiveTo.HasValue)
        {
            throw new DomainRuleException("اختصاص قبلا تمام شده است");
        }

        if (effectiveTo < EffectiveFrom)
        {
            throw new DomainRuleException("تاریخ پایان اختصاص نمی‌تواند مقدم بر تاریخ شروع باشد");
        }

        EffectiveTo = effectiveTo;
    }
}