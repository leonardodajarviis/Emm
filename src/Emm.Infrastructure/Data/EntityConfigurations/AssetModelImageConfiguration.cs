using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class AssetModelImageConfiguration : IEntityTypeConfiguration<AssetModelImage>
{
    public void Configure(EntityTypeBuilder<AssetModelImage> builder)
    {
        builder.ToTable("AssetModelImages");

        // Primary Key
        builder.HasKey(x => new { x.AssetModelId, x.FileId });


        builder.Property(x => x.AssetModelId)
            .IsRequired();

        builder.Property(x => x.FileId)
            .IsRequired();

        builder.Property(x => x.FilePath)
            .IsRequired();

        builder.HasIndex(x => x.FileId)
            .IsUnique();
    }
}
