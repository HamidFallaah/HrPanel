using HrPanel.Domain.Common;

namespace HrPanel.Domain.Organization;

public sealed class JobLevel : BaseEntity<short>
{
    private JobLevel()
    {

    }

    private JobLevel(string code,string titleFa,string? titleEn,short rank)
    {
        SetDetails(code,titleFa,titleEn,rank);
        IsActive = true;
    }
    public string Code { get; private set; } = null!;
    public string TitleFa { get; private set; } = null!;
    public string? TitleEn { get; private set; }
    public short Rank { get; private set; }
    public bool IsActive { get; private set; }

    public static JobLevel Create(string code,string titleFa,string? titleEn,short rank)
    {
        return new JobLevel(code,titleFa,titleEn,rank);
    }

    public void Update(string code,string titleFa,string? titleEn,short rank)
    {
        SetDetails(code,titleFa,titleEn,rank);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void SetDetails(string code,string titleFa,string? titleEn,short rank)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainRuleException("کد سطح شغلی الزامی است");
        }

        if (string.IsNullOrWhiteSpace(titleFa))
        {
            throw new DomainRuleException("عنوان فارسی سطح شغلی الزامی است");
        }

        if (rank < 0)
        {
            throw new DomainRuleException("رتبه سطح شغلی نمی‌تواند منفی باشد");
        }

        Code = code.Trim();
        TitleFa = titleFa.Trim();
        TitleEn = titleEn?.Trim();
        Rank = rank;
    }
}
