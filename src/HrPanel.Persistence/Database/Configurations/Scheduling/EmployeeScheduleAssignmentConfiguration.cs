using HrPanel.Domain.Scheduling;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Scheduling;

public sealed class EmployeeScheduleAssignmentConfiguration : IEntityTypeConfiguration<EmployeeScheduleAssignment>
{
    public void Configure( EntityTypeBuilder<EmployeeScheduleAssignment> builder)
    {
        builder.ToTable("EmployeeScheduleAssignments",DatabaseSchemas.Attendance,tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_EmployeeScheduleAssignments_DateRange",
                    "[EffectiveTo] IS NULL OR " +
                    "[EffectiveTo] >= [EffectiveFrom]");

                tableBuilder.HasCheckConstraint(
                    "CK_EmployeeScheduleAssignments_RotationOffsetDays",
                    "[RotationOffsetDays] >= 0 AND " +
                    "[RotationOffsetDays] <= 365");
            });

        builder.HasKey(assignment => assignment.Id);

        builder.Property(assignment => assignment.Id).UseIdentityColumn();

        builder.Property(assignment => assignment.EmploymentId).IsRequired();

        builder.Property(assignment => assignment.WorkScheduleId).IsRequired();

        builder.Property(assignment => assignment.EffectiveFrom).HasColumnType("date").IsRequired();

        builder.Property(assignment => assignment.EffectiveTo).HasColumnType("date");

        builder.Property(assignment => assignment.RotationOffsetDays).HasColumnType("smallint").HasDefaultValue((short)0).IsRequired();

        builder.Ignore(assignment => assignment.IsCurrent);

        builder.HasOne(assignment => assignment.Employment).WithMany().HasForeignKey(assignment => assignment.EmploymentId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(assignment => assignment.WorkSchedule).WithMany().HasForeignKey(assignment => assignment.WorkScheduleId).OnDelete(DeleteBehavior.Restrict);

        // Each employment can have only one current schedule
        builder.HasIndex(assignment => assignment.EmploymentId).IsUnique().HasFilter("[EffectiveTo] IS NULL").HasDatabaseName( "UX_EmployeeScheduleAssignments_CurrentEmployment");

        builder.HasIndex(assignment => assignment.WorkScheduleId).HasDatabaseName("IX_EmployeeScheduleAssignments_WorkScheduleId");

        builder.HasIndex(assignment => new
        {
            assignment.EmploymentId,
            assignment.EffectiveFrom
        })
            .HasDatabaseName("IX_EmployeeScheduleAssignments_Employment_Date");

        builder.ConfigureAuditProperties();
    }
}