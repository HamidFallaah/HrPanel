using HrPanel.Domain.Employees;
using HrPanel.Persistence.LegacyImport;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations;

internal sealed class LegacyEmployeeImportRowConfiguration: IEntityTypeConfiguration<LegacyEmployeeImportRow>
{
    public void Configure(EntityTypeBuilder<LegacyEmployeeImportRow> builder)
    {
        builder.ToTable("LegacyEmployeeImportRows",DatabaseSchemas.Staging,table =>
            {
                table.HasCheckConstraint("CK_LegacyEmployeeImportRows_SourceRowNumber","[SourceRowNumber] > 0");

                table.HasCheckConstraint("CK_LegacyEmployeeImportRows_ImportStatus","[ImportStatus] IN (1, 2, 3, 4, 5)");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.BatchId).IsRequired();

        builder.Property(x => x.SourceRowNumber).IsRequired();

        builder.Property(x => x.ImportStatus).HasConversion<short>().HasColumnType("smallint").IsRequired();

        builder.Property(x => x.ReceivedAtUtc).HasColumnType("datetime2(7)").IsRequired();

        builder.Property(x => x.ProcessedAtUtc).HasColumnType("datetime2(7)");

        builder.Property(x => x.ErrorDetails).HasColumnType("nvarchar(max)");

        builder.HasIndex(x => new
        {
            x.BatchId,
            x.SourceRowNumber
        })
            .IsUnique().HasDatabaseName("UX_LegacyEmployeeImportRows_Batch_Row");

        builder.HasIndex(x => x.ImportStatus).HasDatabaseName("IX_LegacyEmployeeImportRows_Status");

        builder.HasIndex(x => x.ImportedEmployeeId).HasDatabaseName("IX_LegacyEmployeeImportRows_ImportedEmployeeId");

        builder.HasOne<Employee>().WithMany().HasForeignKey(x => x.ImportedEmployeeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_LegacyEmployeeImportRows_Employees");
    }
}