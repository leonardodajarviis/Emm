using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class AssetTypeParameterConfiguration : IEntityTypeConfiguration<AssetTypeParameter>
{
    public void Configure(EntityTypeBuilder<AssetTypeParameter> builder)
    {
        builder.ToTable("AssetTypeParameters");

        builder.HasKey(x => new { x.AssetTypeId, x.ParameterId });

        builder.Property(x => x.AssetTypeId)
            .IsRequired();
        builder.Property(x => x.ParameterId)
            .IsRequired();

        builder.HasOne<ParameterCatalog>()
            .WithMany()
            .HasForeignKey(x => x.ParameterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
