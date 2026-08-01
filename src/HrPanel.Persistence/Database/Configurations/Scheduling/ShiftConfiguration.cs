using HrPanel.Domain.Scheduling;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Scheduling;

public sealed class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.ToTable("Shifts",DatabaseSchemas.Attendance,
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_Shifts_WorkHours",
                    "[WorkHours] > 0 AND [WorkHours] <= 24");
            });

        builder.HasKey(shift => shift.Id);

        builder.Property(shift => shift.Id).UseIdentityColumn();

        builder.Property(shift => shift.Code).HasMaxLength(50).IsUnicode(false).IsRequired();

        builder.Property(shift => shift.NameFa).HasMaxLength(100).IsUnicode().IsRequired();

        builder.Property(shift => shift.NameEn).HasMaxLength(100).IsUnicode();

        builder.Property(shift => shift.StartTime).HasColumnType("time(0)").IsRequired();

        builder.Property(shift => shift.EndTime).HasColumnType("time(0)").IsRequired();

        builder.Property(shift => shift.WorkHours).HasPrecision(5, 2).IsRequired();

        builder.Property(shift => shift.IsActive).HasDefaultValue(true).IsRequired();

        builder.HasIndex(shift => shift.Code).IsUnique().HasDatabaseName("UX_Shifts_Code");

        builder.ConfigureAuditProperties();
    }
}