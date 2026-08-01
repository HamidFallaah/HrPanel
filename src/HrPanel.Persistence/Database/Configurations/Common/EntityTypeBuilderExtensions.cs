using HrPanel.Persistence.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrPanel.Persistence.Database.Configurations.Common;

// سعی کردم اصل DRY رعایت کنم 
internal static class EntityTypeBuilderExtensions
{
    public static void ConfigureAuditProperties<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class
    {
        builder.Property<DateTime>(AuditPropertyNames.CreatedAt).HasColumnType("datetime2").IsRequired();

        builder.Property<Guid?>(AuditPropertyNames.CreatedByUserId);

        builder.Property<DateTime?>(AuditPropertyNames.ModifiedAt).HasColumnType("datetime2");

        builder.Property<Guid?>(AuditPropertyNames.ModifiedByUserId);

        builder.HasIndex(AuditPropertyNames.CreatedAt);
    }
}
