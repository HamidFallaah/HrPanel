using HrPanel.Domain.Common;

namespace HrPanel.Domain.Organization;

// values can be Company Division ,Subdivision, Department, Section, Unit
public sealed class OrganizationUnitType : BaseEntity<short>
{
    private OrganizationUnitType()
    {

    }

    private OrganizationUnitType(string code,string nameFa,string? nameEn,short hierarchyOrder)
    {
        SetDetails(code,nameFa,nameEn,hierarchyOrder);
        IsActive = true;
    }
    public string Code { get; private set; } = null!;
    public string NameFa { get; private set; } = null!;
    public string? NameEn { get; private set; }
    public short HierarchyOrder { get; private set; }
    public bool IsActive { get; private set; }

    public static OrganizationUnitType Create(string code,string nameFa,string? nameEn,short hierarchyOrder)
    {
        return new OrganizationUnitType(code,nameFa,nameEn,hierarchyOrder);
    }

    public void Update(string code,string nameFa,string? nameEn,short hierarchyOrder)
    {
        SetDetails(code,nameFa,nameEn,hierarchyOrder);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private void SetDetails(string code,string nameFa,string? nameEn,short hierarchyOrder)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainRuleException("کد نوع واحد سازمانی الزامی است");
        }

        if (string.IsNullOrWhiteSpace(nameFa))
        {
            throw new DomainRuleException("نام فارسی نوع واحد سازمانی الزامی است");
        }

        if (hierarchyOrder < 1)
        {
            throw new DomainRuleException("ترتیب سلسله مراتب باید بزرگتر از صفر باشد");
        }

        Code = code.Trim();
        NameFa = nameFa.Trim();
        NameEn = nameEn?.Trim();
        HierarchyOrder = hierarchyOrder;
    }
}
