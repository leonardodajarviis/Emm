using Emm.Domain.Entities.AssetCatalog;
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
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.MaintenancePlanDefinitionId)
            .IsRequired();

        builder.Property(x => x.ItemId)
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

        // Index để tìm kiếm nhanh theo MaintenancePlanDefinitionId
        builder.HasIndex(x => x.MaintenancePlanDefinitionId)
            .HasDatabaseName("IX_MaintenancePlanRequiredItems_MaintenancePlanDefinitionId");

        // Index để tìm kiếm theo ItemId
        builder.HasIndex(x => x.ItemId)
            .HasDatabaseName("IX_MaintenancePlanRequiredItems_ItemId");
    }
}
