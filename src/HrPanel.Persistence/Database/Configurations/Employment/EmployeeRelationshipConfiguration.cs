using HrPanel.Domain.Employment;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Employment;

public sealed class EmployeeRelationshipConfiguration: IEntityTypeConfiguration<EmployeeRelationship>
{
    public void Configure(EntityTypeBuilder<EmployeeRelationship> builder)
    {
        builder.ToTable("EmployeeRelationships",DatabaseSchemas.Hr,tableBuilder =>
            {
                // The check constraints guarantee that a relationship points to exactly one of RelatedEmployeeId , RelatedExternalPersonId
                // It can not point to both or neither, and an employee cannot be related to themselves

                tableBuilder.HasCheckConstraint("CK_EmployeeRelationships_DateRange","[EffectiveTo] IS NULL OR " +"[EffectiveTo] >= [EffectiveFrom]");

                tableBuilder.HasCheckConstraint(
                    "CK_EmployeeRelationships_SingleRelatedParty",
                    "([RelatedEmployeeId] IS NOT NULL AND " +
                    "[RelatedExternalPersonId] IS NULL) OR " +
                    "([RelatedEmployeeId] IS NULL AND " +
                    "[RelatedExternalPersonId] IS NOT NULL)");

                tableBuilder.HasCheckConstraint("CK_EmployeeRelationships_NotSelf","[RelatedEmployeeId] IS NULL OR " +"[RelatedEmployeeId] <> [EmployeeId]");
            });

        builder.HasKey(relationship => relationship.Id);

        builder.Property(relationship => relationship.Id).UseIdentityColumn();

        builder.Property(relationship => relationship.Type).HasConversion<short>().IsRequired();

        builder.Property(relationship => relationship.Context).HasConversion<short>().IsRequired();

        builder.Property(relationship => relationship.EffectiveFrom).HasColumnType("date").IsRequired();

        builder.Property( relationship => relationship.EffectiveTo).HasColumnType("date");

        builder.Ignore(relationship => relationship.IsCurrent);

        builder.HasOne(relationship => relationship.Employee).WithMany().HasForeignKey(relationship => relationship.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne( relationship => relationship.RelatedEmployee).WithMany().HasForeignKey(relationship => relationship.RelatedEmployeeId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(relationship =>relationship.RelatedExternalPerson).WithMany().HasForeignKey(relationship =>relationship.RelatedExternalPersonId).OnDelete(DeleteBehavior.Restrict);

        // One current relationship per type and context

        builder.HasIndex(relationship => new
        {
            relationship.EmployeeId,
            relationship.Type,
            relationship.Context
        }).IsUnique().HasFilter("[EffectiveTo] IS NULL").HasDatabaseName("UX_EmployeeRelationships_CurrentByContext");

        builder.HasIndex(relationship => relationship.RelatedEmployeeId);

        builder.HasIndex(relationship => relationship.RelatedExternalPersonId);

        builder.ConfigureAuditProperties();
    }
}