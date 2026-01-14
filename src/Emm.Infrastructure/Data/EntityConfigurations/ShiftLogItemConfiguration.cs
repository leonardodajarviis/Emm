using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Inventory;
using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class ShiftLogItemConfiguration : IEntityTypeConfiguration<ShiftLogItem>
{
    public void Configure(EntityTypeBuilder<ShiftLogItem> builder)
    {
        builder.ToTable("ShiftLogItems");

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ShiftLogId)
            .IsRequired();

        builder.Property(x => x.ItemId)
            .IsRequired();

        builder.Property(x => x.ItemCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ItemName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(x => x.AssetId);

        builder.Property(x => x.GoodsIssueId)
            .IsRequired(false);

        builder.Property(x => x.GoodsIssueLineId);

        builder.Property(x => x.AssetCode)
            .HasMaxLength(50);

        builder.Property(x => x.AssetName)
            .HasMaxLength(200);

        builder.Property(x => x.UnitOfMeasureId);

        builder.Property(x => x.UnitOfMeasureCode)
            .HasMaxLength(20);

        builder.Property(x => x.UnitOfMeasureName)
            .HasMaxLength(100);

        builder.HasOne<UnitOfMeasure>()
            .WithMany()
            .HasForeignKey(x => x.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Item>()
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Asset>()
            .WithMany()
            .HasForeignKey(x => x.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<GoodsIssue>()
            .WithMany()
            .HasForeignKey(x => x.GoodsIssueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<GoodsIssueLine>()
            .WithMany()
            .HasForeignKey(x => x.GoodsIssueLineId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.ShiftLogId);
        builder.HasIndex(x => x.ItemId);
    }
}
