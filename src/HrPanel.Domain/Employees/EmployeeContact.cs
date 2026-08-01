using HrPanel.Domain.Common;

namespace HrPanel.Domain.Employees;

public sealed class EmployeeContact : AuditableEntity<long>
{
    private EmployeeContact()
    {

    }

    private EmployeeContact(ContactType type,string value,bool isPrimary)
    {
        Type = type;
        SetValue(value);
        IsPrimary = isPrimary;
    }

    public long EmployeeId { get; private set; }
    public ContactType Type { get; private set; }
    public string Value { get; private set; } = null!;
    public bool IsPrimary { get; private set; }
    public Employee Employee { get; private set; } = null!;
    public static EmployeeContact Create(ContactType type,string value,bool isPrimary = false)
    {
        return new EmployeeContact(type, value, isPrimary);
    }
    public void ChangeValue(string value)
    {
        SetValue(value);
    }

    internal void MarkAsPrimary()
    {
        IsPrimary = true;
    }
    internal void MarkAsSecondary()
    {
        IsPrimary = false;
    }
    private void SetValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainRuleException(
                "Contact value is required.");
        }

        Value = value.Trim();
    }
}