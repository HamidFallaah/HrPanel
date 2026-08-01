using HrPanel.Domain.Employment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Employment;

public sealed class WorkTimeTypeConfiguration: IEntityTypeConfiguration<WorkTimeType>
{
    public void Configure(EntityTypeBuilder<WorkTimeType> builder)
    {
        builder.ToTable("WorkTimeTypes",DatabaseSchemas.Hr);

        builder.HasKey(workTimeType => workTimeType.Id);

        builder.Property(workTimeType => workTimeType.Id).UseIdentityColumn();

        builder.Property(workTimeType => workTimeType.Code).HasMaxLength(50).IsUnicode(false).IsRequired();

        builder.Property(workTimeType => workTimeType.NameFa).HasMaxLength(100).IsUnicode().IsRequired();

        builder.Property(workTimeType => workTimeType.NameEn).HasMaxLength(100).IsUnicode();

        builder.Property(workTimeType => workTimeType.IsActive).HasDefaultValue(true).IsRequired();

        builder.HasIndex(workTimeType => workTimeType.Code).IsUnique().HasDatabaseName("UX_WorkTimeTypes_Code");
    }
}