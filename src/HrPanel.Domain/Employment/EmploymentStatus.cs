using HrPanel.Domain.Common;

namespace HrPanel.Domain.Employment;

public sealed class EmploymentStatus : BaseEntity<short>
{
    private EmploymentStatus()
    {

    }

    private EmploymentStatus(string code,string nameFa,string? nameEn)
    {
        SetDetails(code,nameFa,nameEn);
        IsActive = true;
    }
    public string Code { get; private set; } = null!;
    public string NameFa { get; private set; } = null!;
    public string? NameEn { get; private set; }
    public bool IsActive { get; private set; }

    public static EmploymentStatus Create(string code,string nameFa,string? nameEn = null)
    {
        return new EmploymentStatus(code,nameFa,nameEn);
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
            throw new DomainRuleException("کد وضعیت استخدام الزامی است");
        }

        if (string.IsNullOrWhiteSpace(nameFa))
        {
            throw new DomainRuleException("نام فارسی وضعیت استخدام الزامی است");
        }

        Code = code.Trim();
        NameFa = nameFa.Trim();
        NameEn = nameEn?.Trim();
    }
}
