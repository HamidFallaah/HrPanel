using HrPanel.Domain.Employees;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace HrPanel.Persistence.Database.Configurations.Employees;

public sealed class EmployeeContactConfiguration: IEntityTypeConfiguration<EmployeeContact>
{
    public void Configure(EntityTypeBuilder<EmployeeContact> builder)
    {
        builder.ToTable("EmployeeContacts",DatabaseSchemas.Hr);

        builder.HasKey(contact => contact.Id);

        builder.Property(contact => contact.Id).UseIdentityColumn();

        builder.Property(contact => contact.Type).HasConversion<short>().IsRequired();

        builder.Property(contact => contact.Value).HasMaxLength(EmployeeConstants.ContactValueMaxLength).IsUnicode().IsRequired();

        builder.Property(contact => contact.IsPrimary).HasDefaultValue(false).IsRequired();

        builder.HasIndex(contact => contact.EmployeeId).HasDatabaseName("IX_EmployeeContacts_EmployeeId");

        builder.HasIndex(contact => new
        {
            contact.EmployeeId,
            contact.Type
        }).IsUnique().HasFilter("[IsPrimary] = 1").HasDatabaseName("UX_EmployeeContacts_PrimaryByType");

        builder.ConfigureAuditProperties();
    }
   
    //The filtered unique index guarantees that one employee can have

    //Multiple mobile numbers, but only one primary mobile
    //Multiple email addresses, but only one primary email
    //Multiple telephone numbers, but only one primary telephone
}