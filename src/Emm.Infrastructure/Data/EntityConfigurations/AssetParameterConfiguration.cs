using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;


public class AssetParameterConfiguration : IEntityTypeConfiguration<AssetParameter>
{
    public void Configure(EntityTypeBuilder<AssetParameter> builder)
    {
        builder.ToTable("AssetParameters");

        builder.HasKey(ap => new { ap.AssetId, ap.ParameterId });

        builder.Property(ap => ap.CurrentValue)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ap => ap.ValueToMaintenance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne<ParameterCatalog>()
            .WithMany()
            .HasForeignKey(ap => ap.ParameterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
