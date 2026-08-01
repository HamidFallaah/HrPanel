using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using EmploymentEntity = HrPanel.Domain.Employment.Employment;

namespace HrPanel.Persistence.Database.Configurations.Employment;

public sealed class EmploymentConfiguration: IEntityTypeConfiguration<EmploymentEntity>
{
    public void Configure(EntityTypeBuilder<EmploymentEntity> builder)
    {
        builder.ToTable("Employments",DatabaseSchemas.Hr,
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_Employments_DateRange",
                    "[EndDate] IS NULL OR [EndDate] >= [StartDate]");

                tableBuilder.HasCheckConstraint(
                    "CK_Employments_ContractTermMonths",
                    "[ContractTermMonths] IS NULL OR " +
                    "([ContractTermMonths] >= 1 AND " +
                    "[ContractTermMonths] <= 120)");
            });

        builder.HasKey(employment => employment.Id);

        builder.Property(employment => employment.Id).UseIdentityColumn();

        builder.Property(employment => employment.StartDate).HasColumnType("date").IsRequired();

        builder.Property(employment => employment.EndDate).HasColumnType("date");

        builder.Property(employment => employment.ContractTermMonths).HasColumnType("smallint");

        builder.Property(employment => employment.WorkTimeTypeId).HasColumnType("smallint").IsRequired(false);

        builder.Property(employment => employment.TerminationReason).HasMaxLength(1000).IsUnicode();

        builder.Ignore(employment => employment.IsCurrent);

        builder.HasOne(employment => employment.Employee).WithMany().HasForeignKey(employment => employment.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(employment => employment.EmploymentType).WithMany().HasForeignKey(employment => employment.EmploymentTypeId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(employment => employment.EmploymentStatus).WithMany().HasForeignKey(employment => employment.EmploymentStatusId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(employment => employment.WorkTimeType).WithMany().HasForeignKey(employment => employment.WorkTimeTypeId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(employment => employment.Assignments).WithOne(assignment => assignment.Employment).HasForeignKey(assignment => assignment.EmploymentId).OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(employment => employment.Assignments).HasField("_assignments").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(employment => employment.EmployeeId).IsUnique().HasFilter("[EndDate] IS NULL").HasDatabaseName("UX_Employments_CurrentByEmployee");

        builder.HasIndex(employment => employment.EmploymentTypeId).HasDatabaseName("IX_Employments_EmploymentTypeId");

        builder.HasIndex(employment => employment.EmploymentStatusId).HasDatabaseName("IX_Employments_EmploymentStatusId");

        builder.HasIndex(employment => employment.WorkTimeTypeId).HasDatabaseName("IX_Employments_WorkTimeTypeId");

        builder.ConfigureAuditProperties();
    }
}