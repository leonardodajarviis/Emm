using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class MaintenancePlanRequiredItemConfiguration : IEntityTypeConfiguration<MaintenancePlanRequiredItem>
{
    public void Configure(EntityTypeBuilder<MaintenancePlanRequiredItem> builder)
    {
        builder.ToTable("MaintenancePlanRequiredItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.MaintenancePlanDefinitionId)
            .IsRequired();

        builder.Property(x => x.ItemId)
            .IsRequired();

        builder.Property(x => x.UnitOfMeasureId)
            .IsRequired();

        builder.Property(x => x.ItemGroupId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.IsRequired)
            .IsRequired();

        builder.Property(x => x.Note)
            .HasMaxLength(500);

        // Foreign key relationship với MaintenancePlanDefinition
        builder.HasOne<MaintenancePlanDefinition>()
            .WithMany(mpd => mpd.RequiredItems)
            .HasForeignKey(x => x.MaintenancePlanDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Item>()
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<UnitOfMeasure>()
            .WithMany()
            .HasForeignKey(x => x.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<ItemGroup>()
            .WithMany()
            .HasForeignKey(x => x.ItemGroupId)
            .OnDelete(DeleteBehavior.NoAction);

        // Index để tìm kiếm nhanh theo MaintenancePlanDefinitionId
        builder.HasIndex(x => x.MaintenancePlanDefinitionId)
            .HasDatabaseName("IX_MaintenancePlanRequiredItems_MaintenancePlanDefinitionId");

        // Index để tìm kiếm theo ItemId
        builder.HasIndex(x => x.ItemId)
            .HasDatabaseName("IX_MaintenancePlanRequiredItems_ItemId");
    }
}
