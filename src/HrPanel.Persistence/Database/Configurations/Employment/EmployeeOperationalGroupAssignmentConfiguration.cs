using HrPanel.Domain.Employment;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Employment;

public sealed class EmployeeOperationalGroupAssignmentConfiguration: IEntityTypeConfiguration<EmployeeOperationalGroupAssignment>
{
    public void Configure( EntityTypeBuilder<EmployeeOperationalGroupAssignment> builder)
    {
        builder.ToTable("EmployeeOperationalGroupAssignments",DatabaseSchemas.Hr,
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_EmployeeOperationalGroupAssignments_DateRange",
                    "[EffectiveTo] IS NULL OR " +
                    "[EffectiveTo] >= [EffectiveFrom]");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.EmploymentId).IsRequired();

        builder.Property(x => x.OperationalGroupId).IsRequired();

        builder.Property(x => x.EffectiveFrom).HasColumnType("date").IsRequired();

        builder.Property(x => x.EffectiveTo).HasColumnType("date");

        builder.Property(x => x.IsPrimary).HasDefaultValue(true).IsRequired();

        builder.Ignore(x => x.IsCurrent);

        builder.HasOne(x => x.Employment).WithMany().HasForeignKey(x => x.EmploymentId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OperationalGroup).WithMany().HasForeignKey(x => x.OperationalGroupId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.EmploymentId,
            x.OperationalGroupId,
            x.EffectiveFrom
        })
            .IsUnique()
            .HasDatabaseName("UX_EmployeeOperationalGroupAssignments_Membership");

        builder.HasIndex(x => new
        {
            x.EmploymentId,
            x.OperationalGroupId
        })
            .IsUnique()
            .HasFilter("[EffectiveTo] IS NULL")
            .HasDatabaseName("UX_EmployeeOperationalGroupAssignments_CurrentMembership");

        builder.HasIndex(x => x.EmploymentId)
            .IsUnique()
            .HasFilter("[EffectiveTo] IS NULL AND [IsPrimary] = 1")
            .HasDatabaseName("UX_EmployeeOperationalGroupAssignments_CurrentPrimary");

        builder.HasIndex(x => x.OperationalGroupId)
            .HasDatabaseName("IX_EmployeeOperationalGroupAssignments_OperationalGroupId");

        builder.ConfigureAuditProperties();
    }
}