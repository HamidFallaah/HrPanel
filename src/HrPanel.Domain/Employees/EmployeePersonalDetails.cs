using HrPanel.Domain.Common;

namespace HrPanel.Domain.Employees;

public sealed class EmployeePersonalDetails
{
    private EmployeePersonalDetails()
    {

    }

    private EmployeePersonalDetails(string firstNameFa,string lastNameFa,string? nationalCode)
    {
        UpdateNames(firstName: null,lastName: null,firstNameFa,lastNameFa);

        SetNationalCode(nationalCode);
    }

    public long EmployeeId { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string FirstNameFa { get; private set; } = null!;
    public string LastNameFa { get; private set; } = null!;
    public string? NationalCode { get; private set; }
    public string? FatherName { get; private set; }
    public string? FatherNationalCode { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public string? BirthPlace { get; private set; }
    public Gender Gender { get; private set; }
    public MaritalStatus MaritalStatus { get; private set; }
    public Employee Employee { get; private set; } = null!;
    public static EmployeePersonalDetails Create(string firstNameFa,string lastNameFa,string? nationalCode = null)
    {
        return new EmployeePersonalDetails(firstNameFa,lastNameFa,nationalCode);
    }

    public void UpdateNames(string? firstName,string? lastName,string firstNameFa,string lastNameFa)
    {
        if (string.IsNullOrWhiteSpace(firstNameFa))
        {
            throw new DomainRuleException("نام فارسی الزامی است");
        }

        if (string.IsNullOrWhiteSpace(lastNameFa))
        {
            throw new DomainRuleException("نام خانوادگی فارسی الزامی است");
        }

        FirstName = firstName?.Trim();
        LastName = lastName?.Trim();
        FirstNameFa = firstNameFa.Trim();
        LastNameFa = lastNameFa.Trim();
    }

    public void SetNationalCode(string? nationalCode)
    {
        NationalCode = NormalizeNationalCode(nationalCode,"کد ملی");
    }

    private static string? NormalizeNationalCode(string? nationalCode,string fieldName)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            return null;
        }

        nationalCode = nationalCode.Trim();

        if (nationalCode.Length !=EmployeeConstants.NationalCodeLength || nationalCode.Any(character => character < '0' || character > '9'))
        {
            throw new DomainRuleException($"{fieldName} باید دقیقاً ۱۰ رقم باشد");
        }

        return nationalCode;
    }

    public void UpdatePersonalInformation(DateOnly? birthDate,string? birthPlace,Gender gender,MaritalStatus maritalStatus,string? fatherName,string? fatherNationalCode)
    {
        BirthDate = birthDate;
        BirthPlace = birthPlace?.Trim();
        Gender = gender;
        MaritalStatus = maritalStatus;
        FatherName = fatherName?.Trim();
        FatherNationalCode = NormalizeNationalCode(fatherNationalCode,"کد ملی پدر");
    }
}
