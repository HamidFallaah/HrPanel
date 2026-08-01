using HrPanel.Domain.Employees;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Employees;

public sealed class EmployeeConfiguration: IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees",DatabaseSchemas.Hr,
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_Employees_EmployeeNumber_NotEmpty", "LEN(LTRIM(RTRIM([EmployeeNumber]))) > 0");
            });

        builder.HasKey(employee => employee.Id);

        builder.Property(employee => employee.Id).UseIdentityColumn();
        //EmployeeNumber is required and globally unique
        builder.Property(employee => employee.EmployeeNumber).HasMaxLength(EmployeeConstants.EmployeeNumberMaxLength).IsUnicode(false).IsRequired();

        builder.Property(employee => employee.LegacyUserId).HasMaxLength(EmployeeConstants.LegacyUserIdMaxLength).IsUnicode();

        builder.Property(employee => employee.LegacyGuid);

        builder.Property(employee => employee.IsActive).HasDefaultValue(true).IsRequired();

        builder.Property(employee => employee.RowVersion).IsRowVersion().IsConcurrencyToken();

        builder.HasIndex(employee => employee.EmployeeNumber).IsUnique().HasDatabaseName("UX_Employees_EmployeeNumber");

        builder.HasIndex(employee => employee.LegacyUserId).IsUnique().HasFilter("[LegacyUserId] IS NOT NULL").HasDatabaseName("UX_Employees_LegacyUserId");

        builder.HasIndex(employee => employee.LegacyGuid).IsUnique().HasFilter("[LegacyGuid] IS NOT NULL").HasDatabaseName("UX_Employees_LegacyGuid");

        builder.HasOne(employee => employee.PersonalDetails).WithOne(details => details.Employee).HasForeignKey<EmployeePersonalDetails>(details => details.EmployeeId).OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(employee => employee.Contacts).WithOne(contact => contact.Employee).HasForeignKey(contact => contact.EmployeeId).OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(employee => employee.Identifiers).WithOne(identifier => identifier.Employee).HasForeignKey(identifier => identifier.EmployeeId).OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(employee => employee.Dependents).WithOne(dependent => dependent.Employee).HasForeignKey(dependent => dependent.EmployeeId).OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(employee => employee.EducationRecords).WithOne(education => education.Employee).HasForeignKey(education => education.EmployeeId).OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(employee => employee.Contacts).HasField("_contacts").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(employee => employee.Identifiers).HasField("_identifiers").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(employee => employee.Dependents).HasField("_dependents").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(employee => employee.EducationRecords).HasField("_educationRecords").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.ConfigureAuditProperties();
    }
}