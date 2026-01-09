using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class AssetParameterMaintenanceConfiguration : IEntityTypeConfiguration<AssetParameterMaintenance>
{
    public void Configure(EntityTypeBuilder<AssetParameterMaintenance> builder)
    {
        builder.ToTable("AssetParameterMaintenances");

        builder.HasKey(x => new { x.AssetId, x.MaintenancePlanId });

        builder.Property(x => x.ThresholdValue)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.PlusTolerance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.MinusTolerance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne<MaintenancePlanDefinition>()
            .WithMany()
            .HasForeignKey(x => x.MaintenancePlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
