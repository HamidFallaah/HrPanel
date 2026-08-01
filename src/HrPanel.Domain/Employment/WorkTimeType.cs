using HrPanel.Domain.Common;

namespace HrPanel.Domain.Employment;
public sealed class WorkTimeType : BaseEntity<short>
{
    private WorkTimeType()
    {

    }

    private WorkTimeType(string code, string nameFa, string? nameEn)
    {
        SetDetails(code, nameFa, nameEn);
        IsActive = true;
    }

    public string Code { get; private set; } = null!;
    public string NameFa { get; private set; } = null!;
    public string? NameEn { get; private set; }
    public bool IsActive { get; private set; }
    public static WorkTimeType Create(string code, string nameFa, string? nameEn = null)
    {
        return new WorkTimeType(code, nameFa, nameEn);
    }

    public void Update(string code, string nameFa, string? nameEn)
    {
        SetDetails(code, nameFa, nameEn);
    }
    public void Activate()
    {
        IsActive = true;
    }
    public void Deactivate()
    {
        IsActive = false;
    }
    private void SetDetails(string code, string nameFa, string? nameEn)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainRuleException("کد نوع ساعت کاری الزامی است");
        }

        if (string.IsNullOrWhiteSpace(nameFa))
        {
            throw new DomainRuleException("نام فارسی نوع ساعت کاری الزامی است");
        }

        Code = code.Trim().ToUpperInvariant();
        NameFa = nameFa.Trim();

        NameEn = string.IsNullOrWhiteSpace(nameEn) ? null : nameEn.Trim();
    }
}