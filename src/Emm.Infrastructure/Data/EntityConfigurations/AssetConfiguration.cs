using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.AssetTransaction;
using Emm.Domain.Entities.Organization;
using Emm.Domain.ValueObjects;
using Emm.Infrastructure.Data.Converters;
using Emm.Infrastructure.Data.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties Configuration
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Code)
            .HasConversion<NaturalKeyConverter>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasMaxLength(20)
            .HasConversion(
                v => v.Value,
                v => AssetStatus.From(v) // bạn tự implement
            );

        builder.Property(x => x.IsCodeGenerated);
        builder.Property(x => x.IsActive);

        builder.Property(x => x.DisplayName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.HasOne<AssetModel>()
            .WithMany()
            .HasForeignKey(x => x.AssetModelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<AssetAddition>()
            .WithMany()
            .HasForeignKey(x => x.AssetAdditionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<OrganizationUnit>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationUnitId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Location>()
            .WithMany()
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.ConfigureAuditEntity();

        // Indexes
        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasIndex(x => x.DisplayName);

        builder.HasMany(e => e.Parameters)
            .WithOne()
            .HasForeignKey(e => e.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Parameters)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.ParameterMaintenances)
            .WithOne()
            .HasForeignKey(e => e.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.ParameterMaintenances)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
