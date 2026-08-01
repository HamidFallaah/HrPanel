using HrPanel.Domain.Common;

namespace HrPanel.Domain.Employment;

//Employment types and statuses may change, so use database lookup entities instead of enums
public sealed class EmploymentType : BaseEntity<short>
{
    private EmploymentType()
    {

    }

    private EmploymentType(string code,string nameFa,string? nameEn)
    {
        SetDetails(code,nameFa,nameEn);
        IsActive = true;
    }
    public string Code { get; private set; } = null!;
    public string NameFa { get; private set; } = null!;
    public string? NameEn { get; private set; }
    public bool IsActive { get; private set; }

    public static EmploymentType Create(string code,string nameFa,string? nameEn = null)
    {
        return new EmploymentType(code,nameFa,nameEn);
    }

    public void Update(string code,string nameFa,string? nameEn)
    {
        SetDetails(code,nameFa,nameEn);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void SetDetails(string code,string nameFa,string? nameEn)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainRuleException("کد نوع استخدام الزامی است");
        }

        if (string.IsNullOrWhiteSpace(nameFa))
        {
            throw new DomainRuleException("نام فارسی نوع استخدام الزامی است");
        }

        Code = code.Trim();
        NameFa = nameFa.Trim();
        NameEn = nameEn?.Trim();
    }
}
