using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class OperationShiftAssetConfiguration : IEntityTypeConfiguration<OperationShiftAsset>
{
    public void Configure(EntityTypeBuilder<OperationShiftAsset> builder)
    {
        builder.ToTable("OperationShiftAssets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.OperationShiftId)
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
            .IsRequired();

        builder.Property(x => x.AssetBoxId);

        builder.Property(x => x.StartedAt);

        builder.Property(x => x.CompletedAt);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        // Indexes
        builder.HasIndex(x => x.OperationShiftId);

        builder.HasIndex(x => x.AssetId);

        builder.HasIndex(x => x.AssetCode);

        builder.HasIndex(x => x.AssetBoxId);

        // Composite index for unique asset per shift
        builder.HasIndex(x => new { x.OperationShiftId, x.AssetId })
            .IsUnique();
    }
}
