using HrPanel.Domain.Organization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Organization;

public sealed class OrganizationUnitTypeConfiguration: IEntityTypeConfiguration<OrganizationUnitType>
{
    public void Configure(EntityTypeBuilder<OrganizationUnitType> builder)
    {
        builder.ToTable("OrganizationUnitTypes",DatabaseSchemas.Organization,tableBuilder =>
        {
                tableBuilder.HasCheckConstraint("CK_OrganizationUnitTypes_HierarchyOrder","[HierarchyOrder] > 0");
        });

        builder.HasKey(unitType => unitType.Id);

        builder.Property(unitType => unitType.Id).UseIdentityColumn();

        builder.Property(unitType => unitType.Code).HasMaxLength(50).IsUnicode(false).IsRequired();

        builder.Property(unitType => unitType.NameFa).HasMaxLength(100).IsUnicode().IsRequired();

        builder.Property(unitType => unitType.NameEn).HasMaxLength(100).IsUnicode();

        builder.Property(unitType => unitType.HierarchyOrder).HasColumnType("smallint").IsRequired();

        builder.Property(unitType => unitType.IsActive).HasDefaultValue(true).IsRequired();

        builder.HasIndex(unitType => unitType.Code).IsUnique().HasDatabaseName("UX_OrganizationUnitTypes_Code");

        builder.HasIndex(unitType => unitType.HierarchyOrder).HasDatabaseName("IX_OrganizationUnitTypes_HierarchyOrder");
    }
}

// اندیس ‌گذاری شده است اما منحصر به فرد نیست این امر باعث می ‌شود اگر دو نوع واحد سازمانی بعداً به سطح سلسله مراتب یکسانی نیاز داشته باشند، طراحی انعطاف‌ پذیر باشدHierarchyOrder

// This entity inherits from BaseEntity<short>, not AuditableEntity<short>, so do not call: builder.ConfigureAuditProperties();