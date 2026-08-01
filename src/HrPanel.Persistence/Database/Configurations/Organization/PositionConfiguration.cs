using HrPanel.Domain.Organization;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Organization;

public sealed class PositionConfiguration: IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("Positions",DatabaseSchemas.Organization);

        builder.HasKey(position => position.Id);

        builder.Property(position => position.Id).UseIdentityColumn();

        builder.Property(position => position.Code).HasMaxLength(50).IsUnicode(false).IsRequired();

        builder.Property(position => position.TitleFa).HasMaxLength(150).IsUnicode().IsRequired();

        builder.Property(position => position.TitleEn).HasMaxLength(150).IsUnicode();

        builder.Property(position => position.IsActive).HasDefaultValue(true).IsRequired();

        builder.HasIndex(position => position.Code).IsUnique().HasDatabaseName("UX_Positions_Code");

        builder.ConfigureAuditProperties();
    }
}

// A position describes a job title It is not tied directly to an employee here The relationship is created through: hr.EmployeeAssignments.PositionId ->  org.Positions.Id