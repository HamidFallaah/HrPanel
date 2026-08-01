using HrPanel.Domain.Organization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Organization;

public sealed class JobLevelConfiguration: IEntityTypeConfiguration<JobLevel>
{
    public void Configure( EntityTypeBuilder<JobLevel> builder)
    {
        builder.ToTable("JobLevels",DatabaseSchemas.Organization,tableBuilder =>
        {
                tableBuilder.HasCheckConstraint("CK_JobLevels_Rank","[Rank] >= 0");
        });

        builder.HasKey(jobLevel => jobLevel.Id);

        builder.Property(jobLevel => jobLevel.Id).UseIdentityColumn();

        builder.Property(jobLevel => jobLevel.Code).HasMaxLength(50).IsUnicode(false).IsRequired();

        builder.Property(jobLevel => jobLevel.TitleFa).HasMaxLength(100).IsUnicode().IsRequired();

        builder.Property(jobLevel => jobLevel.TitleEn).HasMaxLength(100).IsUnicode();

        builder.Property(jobLevel => jobLevel.Rank).HasColumnType("smallint").IsRequired();

        builder.Property(jobLevel => jobLevel.IsActive).HasDefaultValue(true).IsRequired();

        builder.HasIndex(jobLevel => jobLevel.Code).IsUnique().HasDatabaseName("UX_JobLevels_Code");

        builder.HasIndex(jobLevel => jobLevel.Rank).HasDatabaseName("IX_JobLevels_Rank");
    }
}