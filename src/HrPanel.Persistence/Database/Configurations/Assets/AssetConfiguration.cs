using HrPanel.Domain.Assets;
using HrPanel.Persistence.Database.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Assets;

public sealed class AssetConfiguration: IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets",DatabaseSchemas.Asset);

        builder.HasKey(asset => asset.Id);

        builder.Property(asset => asset.Id).UseIdentityColumn();

        builder.Property(asset => asset.AssetTag).HasMaxLength(100).IsUnicode();

        builder.Property(asset => asset.ServiceNumber).HasMaxLength(100).IsUnicode();

        builder.Property(asset => asset.Imei).HasMaxLength(20).IsUnicode(false);

        builder.Property(asset => asset.SerialNumber).HasMaxLength(100).IsUnicode();

        builder.Property(asset => asset.Status).HasConversion<short>().IsRequired();

        builder.Property(asset => asset.Notes).HasMaxLength(1000).IsUnicode();

        builder.Property(asset => asset.RowVersion).IsRowVersion().IsConcurrencyToken();

        builder.HasOne(asset => asset.AssetType).WithMany().HasForeignKey(asset => asset.AssetTypeId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(asset => asset.AssetTypeId).HasDatabaseName("IX_Assets_AssetTypeId");

        builder.HasIndex(asset => asset.AssetTag).IsUnique().HasFilter("[AssetTag] IS NOT NULL").HasDatabaseName("UX_Assets_AssetTag");

        builder.HasIndex(asset => asset.ServiceNumber).IsUnique().HasFilter("[ServiceNumber] IS NOT NULL").HasDatabaseName("UX_Assets_ServiceNumber");

        builder.HasIndex(asset => asset.Imei).IsUnique().HasFilter("[Imei] IS NOT NULL").HasDatabaseName("UX_Assets_Imei");

        builder.HasIndex(asset => asset.SerialNumber).IsUnique().HasFilter("[SerialNumber] IS NOT NULL").HasDatabaseName("UX_Assets_SerialNumber");

        builder.ConfigureAuditProperties();
    }
}