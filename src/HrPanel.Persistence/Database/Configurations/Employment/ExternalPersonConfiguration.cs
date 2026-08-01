using HrPanel.Domain.Employment;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Employment;

public sealed class ExternalPersonConfiguration: IEntityTypeConfiguration<ExternalPerson>
{
    public void Configure(EntityTypeBuilder<ExternalPerson> builder)
    {
        builder.ToTable("ExternalPersons",DatabaseSchemas.Hr);

        builder.HasKey(person => person.Id);

        builder.Property(person => person.Id).UseIdentityColumn();

        builder.Property(person => person.DisplayName).HasMaxLength(150).IsUnicode().IsRequired();

        builder.Property(person => person.LegacyUsername).HasMaxLength(128).IsUnicode();

        builder.Property(person => person.IsActive).HasDefaultValue(true).IsRequired();

        builder.HasIndex(person => person.LegacyUsername).IsUnique().HasFilter("[LegacyUsername] IS NOT NULL").HasDatabaseName("UX_ExternalPersons_LegacyUsername");

        builder.ConfigureAuditProperties();
    }

    // ExternalPerson مدیران یا سرپرستانی را از سیستم قدیمی که سابقه کارمندی در HrPanel ندارند، حفظ می‌کند
}