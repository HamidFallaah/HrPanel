using HrPanel.Domain.Common;

namespace HrPanel.Domain.Employees;

public sealed class EmployeeDependent : AuditableEntity<long>
{
    private EmployeeDependent()
    {

    }

    private EmployeeDependent(string fullName,DependentRelationshipType relationshipType)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainRuleException("نام وابسته الزامی است");
        }

        FullName = fullName.Trim();
        RelationshipType = relationshipType;
    }

    public long EmployeeId { get; private set; }
    public string FullName { get; private set; } = null!;
    public string? NationalCode { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public DependentRelationshipType RelationshipType { get; private set; }
    public bool IsEmergencyContact { get; private set; }
    public string? EmergencyPhone { get; private set; }
    public Employee Employee { get; private set; } = null!;
    public static EmployeeDependent Create(string fullName,DependentRelationshipType relationshipType)
    {
        return new EmployeeDependent(fullName,relationshipType);
    }
    public void UpdateDetails(string? nationalCode,DateOnly? birthDate)
    {
        if (!string.IsNullOrWhiteSpace(nationalCode))
        {
            nationalCode = nationalCode.Trim();

            if (nationalCode.Length != EmployeeConstants.NationalCodeLength ||
                nationalCode.Any(character => character < '0' || character > '9'))
            {
                throw new DomainRuleException("کد ملی وابسته باید دقیقاً ۱۰ رقم باشد");
            }
        }

        NationalCode = string.IsNullOrWhiteSpace(nationalCode)
            ? null
            : nationalCode;
        BirthDate = birthDate;
    }
    public void SetAsEmergencyContact(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new DomainRuleException("تلفن اضطراری الزامی است");
        }

        IsEmergencyContact = true;
        EmergencyPhone = phone.Trim();
    }

    internal void Update(
        string fullName,
        DependentRelationshipType relationshipType,
        string? nationalCode,
        DateOnly? birthDate,
        bool isEmergencyContact,
        string? emergencyPhone)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainRuleException("نام وابسته الزامی است");
        }

        FullName = fullName.Trim();
        RelationshipType = relationshipType;
        UpdateDetails(nationalCode,birthDate);

        if (isEmergencyContact)
        {
            SetAsEmergencyContact(emergencyPhone ?? string.Empty);
            return;
        }

        IsEmergencyContact = false;
        EmergencyPhone = null;
    }
}
