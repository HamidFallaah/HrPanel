using HrPanel.Domain.Common;

namespace HrPanel.Domain.Employment;

public sealed class Employment : AuditableEntity<long>
{
    private readonly List<EmployeeAssignment> _assignments = [];

    private Employment()
    {

    }

    private Employment(long employeeId,short employmentTypeId,short employmentStatusId,DateOnly startDate,short? contractTermMonths,short? workTimeTypeId)
    {
        ValidateCreation(employeeId,employmentTypeId,employmentStatusId,startDate,contractTermMonths,workTimeTypeId);

        EmployeeId = employeeId;
        EmploymentTypeId = employmentTypeId;
        EmploymentStatusId = employmentStatusId;
        StartDate = startDate;
        ContractTermMonths = contractTermMonths;
        WorkTimeTypeId = workTimeTypeId;
    }

    public long EmployeeId { get; private set; }
    public short EmploymentTypeId { get; private set; }
    public short EmploymentStatusId { get; private set; }
    public short? WorkTimeTypeId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public short? ContractTermMonths { get; private set; }
    public string? TerminationReason { get; private set; }
    public bool IsCurrent => EndDate is null;
    public Employees.Employee Employee { get; private set; } = null!;
    public EmploymentType EmploymentType { get; private set; } = null!;
    public EmploymentStatus EmploymentStatus { get; private set; } = null!;
    public WorkTimeType? WorkTimeType { get; private set; }
    public IReadOnlyCollection<EmployeeAssignment> Assignments => _assignments.AsReadOnly();

    public static Employment Start(long employeeId,short employmentTypeId,short employmentStatusId,DateOnly startDate,short? contractTermMonths = null,short? workTimeTypeId = null)
    {
        return new Employment(employeeId,employmentTypeId,employmentStatusId,startDate,contractTermMonths,workTimeTypeId);
    }

    public void End(DateOnly endDate,short terminatedStatusId,string? reason)
    {
        if (EndDate.HasValue)
        {
            throw new DomainRuleException("استخدام قبلاً تمام شده است");
        }

        if (terminatedStatusId <= 0)
        {
            throw new DomainRuleException("شناسه وضعیت پایان استخدام باید معتبر باشد");
        }

        if (endDate < StartDate)
        {
            throw new DomainRuleException("تاریخ پایان استخدام نمی‌تواند مقدم بر تاریخ شروع باشد");
        }

        if (_assignments.Any(x => x.IsCurrent && x.EffectiveFrom > endDate))
        {
            throw new DomainRuleException("تاریخ پایان استخدام نمی‌تواند مقدم بر تاریخ شروع تخصیص فعال باشد");
        }

        EndDate = endDate;
        EmploymentStatusId = terminatedStatusId;

        TerminationReason = string.IsNullOrWhiteSpace(reason)? null: reason.Trim();

        foreach (var assignment in _assignments.Where(x => x.IsCurrent))
        {
            assignment.End(endDate);
        }
    }

    public void ChangeStatus(short employmentStatusId)
    {
        EnsureEmploymentIsCurrent();

        if (employmentStatusId <= 0)
        {
            throw new DomainRuleException(
                "شناسه وضعیت استخدام باید معتبر باشد");
        }

        EmploymentStatusId = employmentStatusId;
    }

    public void ChangeWorkTimeType(short workTimeTypeId)
    {
        EnsureEmploymentIsCurrent();

        if (workTimeTypeId <= 0)
        {
            throw new DomainRuleException("شناسه نوع ساعت کاری باید معتبر باشد");
        }

        WorkTimeTypeId = workTimeTypeId;
    }

    public void ClearWorkTimeType()
    {
        EnsureEmploymentIsCurrent();
        WorkTimeTypeId = null;
    }

    public void AddAssignment(EmployeeAssignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        EnsureEmploymentIsCurrent();

        if (assignment.EffectiveFrom < StartDate)
        {
            throw new DomainRuleException("تاریخ شروع تخصیص نمی‌تواند مقدم بر تاریخ شروع استخدام باشد");
        }

        if (_assignments.Any(x => x.Context == assignment.Context && x.IsCurrent))
        {
            throw new DomainRuleException("برای این حوزه یک تخصیص فعال وجود دارد");
        }

        _assignments.Add(assignment);
    }

    private void EnsureEmploymentIsCurrent()
    {
        if (EndDate.HasValue)
        {
            throw new DomainRuleException("اطلاعات استخدام پایان‌یافته قابل تغییر نیست");
        }
    }

    private static void ValidateCreation(
        long employeeId,
        short employmentTypeId,
        short employmentStatusId,
        DateOnly startDate,
        short? contractTermMonths,
        short? workTimeTypeId)
    {
        if (employeeId <= 0)
        {
            throw new DomainRuleException("شناسه کارمند باید معتبر باشد");
        }

        if (employmentTypeId <= 0)
        {
            throw new DomainRuleException("شناسه نوع استخدام باید معتبر باشد");
        }

        if (employmentStatusId <= 0)
        {
            throw new DomainRuleException("شناسه وضعیت استخدام باید معتبر باشد");
        }

        if (startDate == default)
        {
            throw new DomainRuleException("تاریخ شروع استخدام الزامی است");
        }

        if (contractTermMonths is <= 0 or > 120)
        {
            throw new DomainRuleException("مدت قرارداد باید بین ۱ تا ۱۲۰ ماه باشد");
        }

        if (workTimeTypeId.HasValue && workTimeTypeId.Value <= 0)
        {
            throw new DomainRuleException("شناسه نوع ساعت کاری باید معتبر باشد");
        }
    }
}