using HrPanel.Domain.Common;

namespace HrPanel.Domain.Organization
{
    public sealed class OrganizationUnit : AuditableEntity<long>
    {
        private readonly List<OrganizationUnit> _children = [];

        private OrganizationUnit()
        {

        }

        private OrganizationUnit(short organizationUnitTypeId,string code,string nameFa,string? nameEn,long? parentOrganizationUnitId)
        {
            OrganizationUnitTypeId = organizationUnitTypeId;
            ParentOrganizationUnitId = parentOrganizationUnitId;
            SetDetails(code,nameFa,nameEn);
            IsActive = true;
        }

        public short OrganizationUnitTypeId { get; private set; }
        public long? ParentOrganizationUnitId { get; private set; }
        public string Code { get; private set; } = null!;
        public string NameFa { get; private set; } = null!;
        public string? NameEn { get; private set; }
        public bool IsActive { get; private set; }
        public OrganizationUnitType OrganizationUnitType { get; private set; } = null!;
        public OrganizationUnit? ParentOrganizationUnit { get; private set; }
        public IReadOnlyCollection<OrganizationUnit> Children => _children;

        public static OrganizationUnit Create(short organizationUnitTypeId,string code,string nameFa,string? nameEn = null,long? parentOrganizationUnitId = null)
        {
            return new OrganizationUnit(organizationUnitTypeId,code,nameFa,nameEn,parentOrganizationUnitId);
        }

        public void Update(short organizationUnitTypeId,string code,string nameFa,string? nameEn)
        {
            OrganizationUnitTypeId = organizationUnitTypeId;
            SetDetails(code,nameFa,nameEn);
        }

        public void MoveTo(long? parentOrganizationUnitId)
        {
            if (parentOrganizationUnitId.HasValue && Id != 0 && parentOrganizationUnitId.Value == Id)
            {
                throw new DomainRuleException("واحد سازمانی نمی‌تواند والد خودش باشد");
            }

            ParentOrganizationUnitId = parentOrganizationUnitId;
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
                throw new DomainRuleException("کد واحد سازمانی الزامی است");
            }

            if (string.IsNullOrWhiteSpace(nameFa))
            {
                throw new DomainRuleException("نام فارسی واحد سازمانی الزامی است");
            }

            Code = code.Trim();
            NameFa = nameFa.Trim();
            NameEn = nameEn?.Trim();
        }
    }
}
