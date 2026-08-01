using HrPanel.Domain.Common;

namespace HrPanel.Domain.Employment;

public sealed class DisciplinaryAction : AuditableEntity<long>
{
    private DisciplinaryAction()
    {

    }

    private DisciplinaryAction(long employeeId,DateOnly startDate,DateOnly? endDate,string details)
    {
        if (string.IsNullOrWhiteSpace(details))
        {
            throw new DomainRuleException("جزئیات اقدامات انضباطی الزامی است");
        }

        if (endDate.HasValue && endDate < startDate)
        {
            throw new DomainRuleException("تاریخ پایان نمی‌ تواند مقدم بر تاریخ شروع باشد");
        }

        EmployeeId = employeeId;
        StartDate = startDate;
        EndDate = endDate;
        Details = details.Trim();
    }
    public long EmployeeId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public string Details { get; private set; } = null!;
    public bool IsClosed => EndDate.HasValue;
    public Employees.Employee Employee { get; private set; } = null!;

    public static DisciplinaryAction Create(long employeeId,DateOnly startDate,DateOnly? endDate,string details)
    {
        return new DisciplinaryAction(employeeId,startDate,endDate,details);
    }

    public void Close(DateOnly endDate)
    {
        if (endDate < StartDate)
        {
            throw new DomainRuleException("تاریخ پایان نمی‌تواند مقدم بر تاریخ شروع باشد");
        }

        EndDate = endDate;
    }
}