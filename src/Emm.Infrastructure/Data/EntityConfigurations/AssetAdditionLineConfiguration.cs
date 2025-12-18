using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.AssetTransaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class AssetAdditionLineConfiguration : IEntityTypeConfiguration<AssetAdditionLine>
{
    public void Configure(EntityTypeBuilder<AssetAdditionLine> builder)
    {
        builder.ToTable("AssetAdditionLines");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties Configuration
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.AssetAdditionId)
            .IsRequired();

        builder.Property(x => x.AssetModelId)
            .IsRequired();

        builder.Property(x => x.AssetCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.AssetAdditionId)
            .HasDatabaseName("IX_AssetAdditionLines_AssetAdditionId");

        builder.HasIndex(x => x.AssetModelId)
            .HasDatabaseName("IX_AssetAdditionLines_AssetModelId");

        builder.HasIndex(x => x.AssetCode)
            .IsUnique()
            .HasDatabaseName("IX_AssetAdditionLines_AssetCode");

        // Composite index for performance
        builder.HasIndex(x => new { x.AssetAdditionId, x.AssetCode })
            .IsUnique()
            .HasDatabaseName("IX_AssetAdditionLines_AssetAdditionId_AssetCode");

        builder.HasOne<AssetModel>()
            .WithMany()
            .HasForeignKey(e => e.AssetModelId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
