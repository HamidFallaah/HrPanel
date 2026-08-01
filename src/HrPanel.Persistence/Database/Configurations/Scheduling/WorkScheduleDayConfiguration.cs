using HrPanel.Domain.Scheduling;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Scheduling;

public sealed class WorkScheduleDayConfiguration : IEntityTypeConfiguration<WorkScheduleDay>
{
    public void Configure( EntityTypeBuilder<WorkScheduleDay> builder)
    {
        builder.ToTable("WorkScheduleDays", DatabaseSchemas.Attendance,
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_WorkScheduleDays_DayIndex",
                    "[DayIndex] >= 0 AND [DayIndex] <= 365");

                tableBuilder.HasCheckConstraint(
                    "CK_WorkScheduleDays_ShiftOrRestDay",
                    "([IsRestDay] = 1 AND [ShiftId] IS NULL) OR " +
                    "([IsRestDay] = 0 AND [ShiftId] IS NOT NULL)");
            });

        builder.HasKey(day => day.Id);

        builder.Property(day => day.Id).UseIdentityColumn();

        builder.Property(day => day.WorkScheduleId).IsRequired();

        builder.Property(day => day.DayIndex).HasColumnType("smallint").IsRequired();

        builder.Property(day => day.ShiftId);

        builder.Property(day => day.IsRestDay).HasDefaultValue(false).IsRequired();

        builder.HasOne(day => day.Shift) .WithMany().HasForeignKey(day => day.ShiftId).OnDelete(DeleteBehavior.Restrict);

        // WorkSchedule relationship is configured in
        // WorkScheduleConfiguration.

        builder.HasIndex(day => new
        {
            day.WorkScheduleId,
            day.DayIndex
        })
            .IsUnique().HasDatabaseName("UX_WorkScheduleDays_Schedule_DayIndex");

        builder.HasIndex(day => day.ShiftId).HasDatabaseName("IX_WorkScheduleDays_ShiftId");

        builder.ConfigureAuditProperties();
    }
}