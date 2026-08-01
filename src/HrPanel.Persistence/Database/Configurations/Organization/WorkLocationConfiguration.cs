using HrPanel.Domain.Organization;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Organization;

public sealed class WorkLocationConfiguration: IEntityTypeConfiguration<WorkLocation>
{
    public void Configure(EntityTypeBuilder<WorkLocation> builder)
    {
        builder.ToTable("WorkLocations",DatabaseSchemas.Organization);

        builder.HasKey(location => location.Id);

        builder.Property(location => location.Id).UseIdentityColumn();

        builder.Property(location => location.Code).HasMaxLength(50).IsUnicode(false).IsRequired();

        builder.Property(location => location.NameFa).HasMaxLength(150).IsUnicode().IsRequired();

        builder.Property(location => location.NameEn).HasMaxLength(150).IsUnicode();

        builder.Property(location => location.Province).HasMaxLength(100).IsUnicode();

        builder.Property(location => location.City).HasMaxLength(100).IsUnicode();

        builder.Property(location => location.Address).HasMaxLength(1000).IsUnicode();

        builder.Property(location => location.IsActive).HasDefaultValue(true).IsRequired();

        builder.HasIndex(location => location.Code).IsUnique().HasDatabaseName("UX_WorkLocations_Code");

        builder.HasIndex(location => new
        {
            location.Province,
            location.City
        }).HasDatabaseName("IX_WorkLocations_Province_City");
        
        // The composite location index helps queries such as
        //SELECT * FROM org.WorkLocations WHERE Province = N'تهران'AND City = N'تهران';

        builder.ConfigureAuditProperties();
    }
}