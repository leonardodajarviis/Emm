using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class ShiftLogAssetConfiguration : IEntityTypeConfiguration<ShiftLogAsset>
{
    public void Configure(EntityTypeBuilder<ShiftLogAsset> builder)
    {
        builder.ToTable("ShiftLogAssets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ShiftLogId)
            .IsRequired();

        builder.Property(x => x.AssetId)
            .IsRequired();

        builder.Property(x => x.AssetCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.AssetName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        // Index
        builder.HasIndex(x => x.ShiftLogId)
            .HasDatabaseName("IX_ShiftLogAssets_ShiftLogId");

        builder.HasIndex(x => x.AssetId)
            .HasDatabaseName("IX_ShiftLogAssets_AssetId");

        // Unique constraint: 1 asset chỉ có thể xuất hiện 1 lần trong 1 ShiftLog
        builder.HasIndex(x => new { x.ShiftLogId, x.AssetId })
            .IsUnique()
            .HasDatabaseName("IX_ShiftLogAssets_ShiftLogId_AssetId");
    }
}
