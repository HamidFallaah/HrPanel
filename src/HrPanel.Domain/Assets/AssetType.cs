using HrPanel.Domain.Common;

namespace HrPanel.Domain.Assets;
// Initial values: TD-LTE Modem , SIM Card , Laptop , Access Card , Mobile Phone, Other
public sealed class AssetType : BaseEntity<short>
{
    private AssetType()
    {

    }
    private AssetType(string code,string nameFa,string? nameEn)
    {
        SetDetails(code,nameFa,nameEn);
        IsActive = true;
    }
    public string Code { get; private set; } = null!;
    public string NameFa { get; private set; } = null!;
    public string? NameEn { get; private set; }
    public bool IsActive { get; private set; }
    public static AssetType Create(string code,string nameFa,string? nameEn = null)
    {
        return new AssetType(code,nameFa,nameEn);
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
            throw new DomainRuleException("کد نوع دارایی الزامی است");
        }

        if (string.IsNullOrWhiteSpace(nameFa))
        {
            throw new DomainRuleException("نام فارسی نوع دارایی الزامی است");
        }

        Code = code.Trim();
        NameFa = nameFa.Trim();
        NameEn = nameEn?.Trim();
    }
}
