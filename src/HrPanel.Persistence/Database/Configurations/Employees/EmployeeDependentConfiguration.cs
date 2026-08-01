using HrPanel.Domain.Employees;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Employees;

public sealed class EmployeeDependentConfiguration: IEntityTypeConfiguration<EmployeeDependent>
{
    public void Configure(EntityTypeBuilder<EmployeeDependent> builder)
    {
        builder.ToTable("EmployeeDependents",DatabaseSchemas.Hr);

        builder.HasKey(dependent => dependent.Id);

        builder.Property(dependent => dependent.Id).UseIdentityColumn();

        builder.Property(dependent => dependent.FullName).HasMaxLength(150).IsUnicode().IsRequired();

        builder.Property(dependent => dependent.NationalCode).HasMaxLength(EmployeeConstants.NationalCodeLength).IsFixedLength().IsUnicode(false);

        builder.Property(dependent => dependent.BirthDate).HasColumnType("date");

        //RelationshipType is stored as smallint, corresponding to
        //Spouse = 1
        //Child = 2
        //Father = 3
        //Mother = 4
        //Other = 5

        builder.Property(dependent => dependent.RelationshipType).HasConversion<short>().IsRequired();

        builder.Property(dependent => dependent.IsEmergencyContact).HasDefaultValue(false).IsRequired();

        builder.Property(dependent => dependent.EmergencyPhone).HasMaxLength(30).IsUnicode(false);

        builder.HasIndex(dependent => dependent.EmployeeId).HasDatabaseName("IX_EmployeeDependents_EmployeeId");

        builder.ConfigureAuditProperties();
    }
}