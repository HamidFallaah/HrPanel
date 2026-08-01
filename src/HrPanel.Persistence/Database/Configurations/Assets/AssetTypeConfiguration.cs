using HrPanel.Domain.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Assets;
public sealed class AssetTypeConfiguration: IEntityTypeConfiguration<AssetType>
{
    public void Configure(EntityTypeBuilder<AssetType> builder)
    {
        builder.ToTable("AssetTypes",DatabaseSchemas.Asset);

        builder.HasKey(assetType => assetType.Id);

        builder.Property(assetType => assetType.Id).UseIdentityColumn();

        builder.Property(assetType => assetType.Code).HasMaxLength(50).IsUnicode(false).IsRequired();

        builder.Property(assetType => assetType.NameFa).HasMaxLength(100).IsUnicode().IsRequired();

        builder.Property(assetType => assetType.NameEn).HasMaxLength(100).IsUnicode();

        builder.Property(assetType => assetType.IsActive).HasDefaultValue(true).IsRequired();

        builder.HasIndex(assetType => assetType.Code).IsUnique().HasDatabaseName("UX_AssetTypes_Code");
    }
}