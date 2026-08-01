using HrPanel.Domain.Common;

namespace HrPanel.Domain.Organization;

public sealed class WorkLocation : AuditableEntity<long>
{
    private WorkLocation()
    {

    }

    private WorkLocation(string code,string nameFa,string? nameEn,string? province,string? city,string? address)
    {
        SetDetails(code,nameFa,nameEn,province,city,address);
        IsActive = true;
    }
    public string Code { get; private set; } = null!;
    public string NameFa { get; private set; } = null!;
    public string? NameEn { get; private set; }
    public string? Province { get; private set; }
    public string? City { get; private set; }
    public string? Address { get; private set; }
    public bool IsActive { get; private set; }

    public static WorkLocation Create(string code,string nameFa,string? nameEn = null,string? province = null,string? city = null,string? address = null)
    {
        return new WorkLocation(code,nameFa,nameEn,province,city,address);
    }

    public void Update(string code,string nameFa,string? nameEn,string? province,string? city,string? address)
    {
        SetDetails(code,nameFa,nameEn,province,city,address);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void SetDetails(string code,string nameFa,string? nameEn,string? province,string? city,string? address)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainRuleException("کد محل کار الزامی است");
        }

        if (string.IsNullOrWhiteSpace(nameFa))
        {
            throw new DomainRuleException("نام فارسی محل کار الزامی است");
        }

        Code = code.Trim();
        NameFa = nameFa.Trim();
        NameEn = nameEn?.Trim();
        Province = province?.Trim();
        City = city?.Trim();
        Address = address?.Trim();
    }
}
