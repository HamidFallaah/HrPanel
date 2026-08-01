using HrPanel.Domain.Employment;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Employment;

public sealed class EmployeeAssignmentConfiguration: IEntityTypeConfiguration<EmployeeAssignment>
{
    public void Configure(EntityTypeBuilder<EmployeeAssignment> builder)
    {
        builder.ToTable("EmployeeAssignments", DatabaseSchemas.Hr, tableBuilder =>
        {
            tableBuilder.HasCheckConstraint("CK_EmployeeAssignments_DateRange", "[EffectiveTo] IS NULL OR " + "[EffectiveTo] >= [EffectiveFrom]");
        });

        builder.HasKey(assignment => assignment.Id);

        builder.Property(assignment => assignment.Id).UseIdentityColumn();

        builder.Property(assignment => assignment.Context).HasConversion<short>().IsRequired();

        builder.Property(assignment => assignment.EffectiveFrom).HasColumnType("date").IsRequired();

        builder.Property(assignment => assignment.EffectiveTo).HasColumnType("date");

        builder.Ignore(assignment => assignment.IsCurrent);

        builder.HasOne(assignment => assignment.OrganizationUnit).WithMany().HasForeignKey(assignment => assignment.OrganizationUnitId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(assignment => assignment.Position).WithMany().HasForeignKey(assignment => assignment.PositionId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(assignment => assignment.JobLevel).WithMany().HasForeignKey(assignment => assignment.JobLevelId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(assignment => assignment.WorkLocation).WithMany().HasForeignKey(assignment => assignment.WorkLocationId).OnDelete(DeleteBehavior.Restrict);

        // One active assignment per context for an employment

        builder.HasIndex(assignment => new
        {
            assignment.EmploymentId,
            assignment.Context
        }).IsUnique().HasFilter("[EffectiveTo] IS NULL").HasDatabaseName("UX_EmployeeAssignments_CurrentByContext");

        builder.HasIndex(assignment => assignment.OrganizationUnitId);

        builder.HasIndex(assignment => assignment.PositionId);

        builder.HasIndex(assignment => assignment.JobLevelId);

        builder.HasIndex(assignment => assignment.WorkLocationId);

        builder.ConfigureAuditProperties();
    }
    // This creates relationships such as
    //hr.EmployeeAssignments.OrganizationUnitId ->  org.OrganizationUnits.Id
}