using HrPanel.Domain.Scheduling;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Scheduling;

public sealed class WorkScheduleConfiguration : IEntityTypeConfiguration<WorkSchedule>
{
    public void Configure(EntityTypeBuilder<WorkSchedule> builder)
    {
        builder.ToTable("WorkSchedules", DatabaseSchemas.Attendance,
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_WorkSchedules_PatternType",
                    "[PatternType] IN (1, 2, 3)");

                tableBuilder.HasCheckConstraint(
                    "CK_WorkSchedules_CycleLengthDays",
                    "[CycleLengthDays] >= 1 AND " +
                    "[CycleLengthDays] <= 366");

                tableBuilder.HasCheckConstraint(
                    "CK_WorkSchedules_RotatingAnchorDate",
                    "[PatternType] <> 2 OR " +
                    "[AnchorDate] IS NOT NULL");
            });

        builder.HasKey(schedule => schedule.Id);

        builder.Property(schedule => schedule.Id).UseIdentityColumn();

        builder.Property(schedule => schedule.Code).HasMaxLength(50).IsUnicode(false).IsRequired();

        builder.Property(schedule => schedule.NameFa).HasMaxLength(150).IsUnicode().IsRequired();

        builder.Property(schedule => schedule.NameEn).HasMaxLength(150).IsUnicode();

        builder.Property(schedule => schedule.PatternType).HasConversion<int>().IsRequired();

        builder.Property(schedule => schedule.CycleLengthDays).HasColumnType("smallint").IsRequired();

        builder.Property(schedule => schedule.AnchorDate).HasColumnType("date");

        builder.Property(schedule => schedule.IsActive).HasDefaultValue(true).IsRequired();

        builder.HasMany(schedule => schedule.Days).WithOne(day => day.WorkSchedule).HasForeignKey(day => day.WorkScheduleId).OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(schedule => schedule.Days).HasField("_days").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(schedule => schedule.Code).IsUnique().HasDatabaseName("UX_WorkSchedules_Code");

        builder.ConfigureAuditProperties();
    }
}