using HrPanel.Domain.Organization;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Organization;

public sealed class OperationalGroupConfiguration: IEntityTypeConfiguration<OperationalGroup>
{
    public void Configure(EntityTypeBuilder<OperationalGroup> builder)
    {
        builder.ToTable("OperationalGroups", DatabaseSchemas.Organization);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.Code).HasMaxLength(50).IsUnicode(false).IsRequired();

        builder.Property(x => x.Name).HasMaxLength(150).IsUnicode(true).IsRequired();

        builder.Property(x => x.Type).HasConversion<short>().IsRequired();

        builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();

        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_OperationalGroups_Code");

        builder.HasIndex(x => new
        {
            x.Type,
            x.IsActive
        })
            .HasDatabaseName("IX_OperationalGroups_Type_IsActive");

        builder.ConfigureAuditProperties();
    }
}