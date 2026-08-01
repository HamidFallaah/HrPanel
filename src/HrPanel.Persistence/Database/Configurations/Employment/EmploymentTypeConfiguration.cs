using HrPanel.Domain.Employment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Employment;

public sealed class EmploymentTypeConfiguration: IEntityTypeConfiguration<EmploymentType>
{
    public void Configure(EntityTypeBuilder<EmploymentType> builder)
    {
        builder.ToTable("EmploymentTypes",DatabaseSchemas.Hr);

        builder.HasKey(employmentType => employmentType.Id);

        builder.Property(employmentType => employmentType.Id).UseIdentityColumn();

        builder.Property(employmentType => employmentType.Code).HasMaxLength(50).IsUnicode(false).IsRequired();

        builder.Property(employmentType => employmentType.NameFa).HasMaxLength(100).IsUnicode().IsRequired();

        builder.Property(employmentType => employmentType.NameEn).HasMaxLength(100).IsUnicode();

        builder.Property(employmentType => employmentType.IsActive).HasDefaultValue(true).IsRequired();

        builder.HasIndex(employmentType => employmentType.Code).IsUnique().HasDatabaseName("UX_EmploymentTypes_Code");

        // This is a lookup entity so it does not use ConfigureAuditProperties()
    }
}