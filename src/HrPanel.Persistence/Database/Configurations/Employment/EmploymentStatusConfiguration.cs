using HrPanel.Domain.Employment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Employment;

public sealed class EmploymentStatusConfiguration: IEntityTypeConfiguration<EmploymentStatus>
{
    public void Configure(EntityTypeBuilder<EmploymentStatus> builder)
    {
        builder.ToTable("EmploymentStatuses",DatabaseSchemas.Hr);

        builder.HasKey(employmentStatus => employmentStatus.Id);

        builder.Property(employmentStatus => employmentStatus.Id).UseIdentityColumn();

        builder.Property(employmentStatus => employmentStatus.Code).HasMaxLength(50).IsUnicode(false).IsRequired();

        builder.Property(employmentStatus => employmentStatus.NameFa).HasMaxLength(100).IsUnicode().IsRequired();

        builder.Property(employmentStatus => employmentStatus.NameEn).HasMaxLength(100).IsUnicode();

        builder.Property(employmentStatus => employmentStatus.IsActive).HasDefaultValue(true).IsRequired();

        builder.HasIndex(employmentStatus => employmentStatus.Code).IsUnique().HasDatabaseName("UX_EmploymentStatuses_Code");
    }
}