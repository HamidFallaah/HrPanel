using HrPanel.Domain.Employees;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Employees;

public sealed class EmployeeIdentifierConfiguration: IEntityTypeConfiguration<EmployeeIdentifier>
{
    public void Configure(EntityTypeBuilder<EmployeeIdentifier> builder)
    {
        builder.ToTable("EmployeeIdentifiers",DatabaseSchemas.Hr);

        builder.HasKey(identifier => identifier.Id);

        builder.Property(identifier => identifier.Id).UseIdentityColumn();

        builder.Property(identifier => identifier.Type).HasConversion<short>().IsRequired();

        builder.Property(identifier => identifier.Value).HasMaxLength(EmployeeConstants.IdentifierValueMaxLength).IsUnicode().IsRequired();

        builder.Property(identifier => identifier.EffectiveFrom).HasColumnType("date");

        builder.Property(identifier => identifier.EffectiveTo).HasColumnType("date");

        builder.HasIndex(identifier => identifier.EmployeeId).HasDatabaseName("IX_EmployeeIdentifiers_EmployeeId");

        builder.HasIndex(identifier => new
        {
            identifier.Type,
            identifier.Value
        }).IsUnique().HasFilter("[EffectiveTo] IS NULL").HasDatabaseName("UX_EmployeeIdentifiers_ActiveValue");

        builder.ConfigureAuditProperties();
    }

    //This supports identifiers such as:
    //AccessCard
    //ArchiveNumber
    //FoodCode
    //StaffNumber
    //InsuranceNumber
    //AttendanceCode
}