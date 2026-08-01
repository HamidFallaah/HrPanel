using HrPanel.Domain.Common;

namespace HrPanel.Domain.Organization;

public sealed class Position : AuditableEntity<long>
{
    private Position()
    {

    }

    private Position(string code,string titleFa,string? titleEn)
    {
        SetDetails(code,titleFa,titleEn);
        IsActive = true;
    }
    public string Code { get; private set; } = null!;
    public string TitleFa { get; private set; } = null!;
    public string? TitleEn { get; private set; }
    public bool IsActive { get; private set; }

    public static Position Create(string code,string titleFa,string? titleEn = null)
    {
        return new Position(code,titleFa,titleEn);
    }

    public void Update(string code,string titleFa,string? titleEn)
    {
        SetDetails(code,titleFa,titleEn);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void SetDetails(string code,string titleFa,string? titleEn)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainRuleException("کد سمت شغلی الزامی است");
        }

        if (string.IsNullOrWhiteSpace(titleFa))
        {
            throw new DomainRuleException("عنوان فارسی سمت شغلی الزامی است");
        }

        Code = code.Trim();
        TitleFa = titleFa.Trim();
        TitleEn = titleEn?.Trim();
    }
}
