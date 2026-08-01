using HrPanel.Domain.Employment;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Employment;

public sealed class DisciplinaryActionConfiguration: IEntityTypeConfiguration<DisciplinaryAction>
{
    public void Configure(EntityTypeBuilder<DisciplinaryAction> builder)
    {
        builder.ToTable("DisciplinaryActions",DatabaseSchemas.Hr,tableBuilder =>
        {
                tableBuilder.HasCheckConstraint("CK_DisciplinaryActions_DateRange","[EndDate] IS NULL OR " + "[EndDate] >= [StartDate]");
        });

        builder.HasKey(action => action.Id);

        builder.Property(action => action.Id).UseIdentityColumn();

        builder.Property(action => action.StartDate).HasColumnType("date").IsRequired();

        builder.Property(action => action.EndDate).HasColumnType("date");

        builder.Property(action => action.Details).HasMaxLength(2000).IsUnicode().IsRequired();

        builder.Ignore(action => action.IsClosed);

        builder.HasOne(action => action.Employee).WithMany().HasForeignKey(action => action.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(action => new
        {
            action.EmployeeId,
            action.StartDate
        }).HasDatabaseName("IX_DisciplinaryActions_Employee_Date");

        builder.ConfigureAuditProperties();
    }
}