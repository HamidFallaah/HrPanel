using HrPanel.Domain.Organization;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Organization;

public sealed class OrganizationUnitConfiguration: IEntityTypeConfiguration<OrganizationUnit>
{
    public void Configure(EntityTypeBuilder<OrganizationUnit> builder)
    {
        builder.ToTable("OrganizationUnits",DatabaseSchemas.Organization,tableBuilder =>
        {
            // محدودیت بررسی مانع از آن می‌ شود که یک واحد، والد مستقیم خودش باشد

            tableBuilder.HasCheckConstraint(
                    "CK_OrganizationUnits_NotOwnParent",
                    "[ParentOrganizationUnitId] IS NULL OR " +
                    "[ParentOrganizationUnitId] <> [Id]");
        });

        builder.HasKey(unit => unit.Id);

        builder.Property(unit => unit.Id).UseIdentityColumn();

        builder.Property(unit => unit.OrganizationUnitTypeId).HasColumnType("smallint").IsRequired();

        builder.Property(unit => unit.Code).HasMaxLength(50).IsUnicode(false).IsRequired();

        builder.Property(unit => unit.NameFa).HasMaxLength(150).IsUnicode().IsRequired();

        builder.Property(unit => unit.NameEn).HasMaxLength(150).IsUnicode();

        builder.Property(unit => unit.IsActive).HasDefaultValue(true).IsRequired();

        // DeleteBehavior.Restrict از حذف جلوگیری می‌کند
        // نوع سازمانی که در حال استفاده است
        // واحد سازمانی والدی که هنوز فرزند دارد

        builder.HasOne(unit => unit.OrganizationUnitType).WithMany().HasForeignKey( unit => unit.OrganizationUnitTypeId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(unit => unit.ParentOrganizationUnit).WithMany(parent => parent.Children).HasForeignKey(unit => unit.ParentOrganizationUnitId).OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(unit => unit.Children).HasField("_children").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(unit => unit.Code).IsUnique().HasDatabaseName("UX_OrganizationUnits_Code");

        builder.HasIndex(unit => unit.OrganizationUnitTypeId).HasDatabaseName("IX_OrganizationUnits_TypeId");

        builder.HasIndex(unit => unit.ParentOrganizationUnitId).HasDatabaseName("IX_OrganizationUnits_ParentId");

        builder.ConfigureAuditProperties();
    }
}

// This mapping creates a self referencing organization hierarchy

// Company -> Division -> Subdivision -> Department ->  Section ->  Unit