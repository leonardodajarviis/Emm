using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class AssetModelParameterConfiguration : IEntityTypeConfiguration<AssetModelParameter>
{
    public void Configure(EntityTypeBuilder<AssetModelParameter> builder)
    {
        builder.ToTable("AssetModelParameters");

        builder.HasKey(x => new { x.AssetModelId, x.ParameterId });

        builder.Property(x => x.AssetModelId)
            .IsRequired();
        builder.Property(x => x.ParameterId)
            .IsRequired();

        builder.HasOne<ParameterCatalog>()
            .WithMany()
            .HasForeignKey(x => x.ParameterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
