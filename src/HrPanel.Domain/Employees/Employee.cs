using HrPanel.Domain.Common;

namespace HrPanel.Domain.Employees;

public sealed class Employee : AuditableEntity<long>
{
    private readonly List<EmployeeContact> _contacts = [];
    private readonly List<EmployeeIdentifier> _identifiers = [];
    private readonly List<EmployeeDependent> _dependents = [];
    private readonly List<EmployeeEducation> _educationRecords = [];

    private Employee()
    {

    }

    private Employee(string employeeNumber,string? legacyUserId,Guid? legacyGuid)
    {
        SetEmployeeNumber(employeeNumber);
        LegacyUserId = legacyUserId?.Trim();
        LegacyGuid = legacyGuid;
        IsActive = true;
    }

    // Setter ها رو public نزاریم چون اونطوری ممکنه قوانین دور زده بشه 
    public string EmployeeNumber { get; private set; } = null!; 
    public string? LegacyUserId { get; private set; }
    public Guid? LegacyGuid { get; private set; }
    public bool IsActive { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public EmployeePersonalDetails? PersonalDetails { get; private set; }
    public IReadOnlyCollection<EmployeeContact> Contacts => _contacts;
    public IReadOnlyCollection<EmployeeIdentifier> Identifiers => _identifiers;
    public IReadOnlyCollection<EmployeeDependent> Dependents => _dependents;
    public IReadOnlyCollection<EmployeeEducation> EducationRecords => _educationRecords;

    public static Employee Create(string employeeNumber,string? legacyUserId = null,Guid? legacyGuid = null)
    {
        return new Employee(employeeNumber,legacyUserId,legacyGuid);
    }

    public void ChangeEmployeeNumber(string employeeNumber)
    {
        SetEmployeeNumber(employeeNumber);
    }

    public void SetLegacyUserId(string? legacyUserId)
    {
        LegacyUserId = string.IsNullOrWhiteSpace(legacyUserId)? null: legacyUserId.Trim();
    }

    public void SetPersonalDetails(EmployeePersonalDetails personalDetails)
    {
        PersonalDetails = personalDetails?? throw new DomainRuleException("اطلاعات شخصی نمی‌تواند خالی باشد");
    }

    public void AddContact(EmployeeContact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);

        if (contact.IsPrimary)
        {
            foreach (var existingContact in _contacts
                         .Where(x => x.Type == contact.Type))
            {
                existingContact.MarkAsSecondary();
            }
        }

        _contacts.Add(contact);
    }

    public void AddIdentifier(EmployeeIdentifier identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier);
        _identifiers.Add(identifier);
    }

    public void UpdateContact(EmployeeContact contact,string value)
    {
        ArgumentNullException.ThrowIfNull(contact);

        if (!_contacts.Contains(contact))
        {
            throw new DomainRuleException("این راه ارتباطی متعلق به کارمند نیست");
        }

        contact.ChangeValue(value);
    }

    public void RemoveContact(EmployeeContact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);

        if (!_contacts.Remove(contact))
        {
            throw new DomainRuleException("این راه ارتباطی متعلق به کارمند نیست");
        }
    }

    public void EndIdentifier(EmployeeIdentifier identifier,DateOnly effectiveTo)
    {
        ArgumentNullException.ThrowIfNull(identifier);

        if (!_identifiers.Contains(identifier))
        {
            throw new DomainRuleException("این شناسه متعلق به کارمند نیست");
        }

        identifier.End(effectiveTo);
    }

    public void AddDependent(EmployeeDependent dependent)
    {
        ArgumentNullException.ThrowIfNull(dependent);
        _dependents.Add(dependent);
    }

    public void AddEducation(EmployeeEducation education)
    {
        ArgumentNullException.ThrowIfNull(education);

        if (education.IsHighestDegree)
        {
            foreach (var record in _educationRecords)
            {
                record.RemoveHighestDegreeFlag();
            }
        }

        _educationRecords.Add(education);
    }

    public void UpdateEducation(
        EmployeeEducation education,
        string? degreeTitle,
        string? fieldOfStudy,
        string? institutionName,
        DateOnly? graduationDate,
        bool isHighestDegree)
    {
        ArgumentNullException.ThrowIfNull(education);

        if (!_educationRecords.Contains(education))
        {
            throw new DomainRuleException("این سابقه تحصیلی متعلق به کارمند نیست");
        }

        education.Update(
            degreeTitle,
            fieldOfStudy,
            institutionName,
            graduationDate);

        if (isHighestDegree)
        {
            MarkEducationAsHighest(education);
        }
        else
        {
            education.RemoveHighestDegreeFlag();
        }
    }

    public void MarkEducationAsHighest(EmployeeEducation education)
    {
        ArgumentNullException.ThrowIfNull(education);

        if (!_educationRecords.Contains(education))
        {
            throw new DomainRuleException("این سابقه تحصیلی متعلق به کارمند نیست");
        }

        foreach (var record in _educationRecords)
        {
            record.RemoveHighestDegreeFlag();
        }

        education.MarkAsHighestDegree();
    }

    public void RemoveEducation(EmployeeEducation education)
    {
        ArgumentNullException.ThrowIfNull(education);

        if (!_educationRecords.Remove(education))
        {
            throw new DomainRuleException("این سابقه تحصیلی متعلق به کارمند نیست");
        }
    }

    public void UpdateDependent(
        EmployeeDependent dependent,
        string fullName,
        DependentRelationshipType relationshipType,
        string? nationalCode,
        DateOnly? birthDate,
        bool isEmergencyContact,
        string? emergencyPhone)
    {
        ArgumentNullException.ThrowIfNull(dependent);

        if (!_dependents.Contains(dependent))
        {
            throw new DomainRuleException("این وابسته متعلق به کارمند نیست");
        }

        dependent.Update(
            fullName,
            relationshipType,
            nationalCode,
            birthDate,
            isEmergencyContact,
            emergencyPhone);
    }

    public void RemoveDependent(EmployeeDependent dependent)
    {
        ArgumentNullException.ThrowIfNull(dependent);

        if (!_dependents.Remove(dependent))
        {
            throw new DomainRuleException("این وابسته متعلق به کارمند نیست");
        }
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void SetEmployeeNumber(string employeeNumber)
    {
        if (string.IsNullOrWhiteSpace(employeeNumber))
        {
            throw new DomainRuleException("شماره پرسنلی الزامی است");
        }

        employeeNumber = employeeNumber.Trim();

        if (employeeNumber.Length >
            EmployeeConstants.EmployeeNumberMaxLength)
        {
            throw new DomainRuleException("شماره پرسنلی بیش از حد مجاز است");
        }

        EmployeeNumber = employeeNumber;
    }

    // در غیر این صورت، تماس‌ گیرندگان می ‌توانند مستقیماً چندین شماره تلفن را به عنوان شماره اصلی علامت‌ گذاری کنند، بدون اینکه از بخش کارمند عبور کنند
    public void MarkContactAsPrimary(EmployeeContact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);

        if (!_contacts.Contains(contact))
        {
            throw new DomainRuleException("این راه ارتباطی متعلق به کارمند نیست");
        }

        foreach (var existingContact in _contacts.Where(x => x.Type == contact.Type))
        {
            existingContact.MarkAsSecondary();
        }

        contact.MarkAsPrimary();
    }
}
