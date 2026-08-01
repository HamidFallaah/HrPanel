using HrPanel.Domain.Assets;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Assets;

public sealed class EmployeeAssetAssignmentConfiguration: IEntityTypeConfiguration<EmployeeAssetAssignment>
{
    public void Configure(EntityTypeBuilder<EmployeeAssetAssignment> builder)
    {
        builder.ToTable("EmployeeAssetAssignments",DatabaseSchemas.Asset,tableBuilder =>
        {
                tableBuilder.HasCheckConstraint(
                    "CK_EmployeeAssetAssignments_DateRange",
                    "[ReturnedAt] IS NULL OR " +
                    "[ReturnedAt] >= [AssignedAt]");
        });

        builder.HasKey(assignment => assignment.Id);

        builder.Property(assignment => assignment.Id).UseIdentityColumn();

        builder.Property(assignment => assignment.AssignedAt).HasColumnType("date").IsRequired();

        builder.Property(assignment => assignment.ReturnedAt).HasColumnType("date");

        builder.Property(assignment => assignment.Notes).HasMaxLength(1000).IsUnicode();

        builder.Ignore(assignment => assignment.IsActive);

        builder.HasOne(assignment => assignment.Asset).WithMany().HasForeignKey(assignment => assignment.AssetId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(assignment => assignment.Employee).WithMany().HasForeignKey(assignment => assignment.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        // An asset can have only one active assignment
        builder.HasIndex(assignment => assignment.AssetId).IsUnique().HasFilter("[ReturnedAt] IS NULL").HasDatabaseName(
                "UX_EmployeeAssetAssignments_CurrentAsset");

        // An employee can simultaneously possess multiple assets
        builder.HasIndex(assignment => assignment.EmployeeId).HasDatabaseName("IX_EmployeeAssetAssignments_EmployeeId");

        builder.ConfigureAuditProperties();
    }
}