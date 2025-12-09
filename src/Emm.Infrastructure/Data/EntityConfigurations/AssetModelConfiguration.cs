using Emm.Domain.Entities.AssetCatalog;
using Emm.Infrastructure.Data.Converters;
using Emm.Infrastructure.Data.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class AssetModelConfiguration : IEntityTypeConfiguration<AssetModel>
{
    public void Configure(EntityTypeBuilder<AssetModel> builder)
    {
        builder.ToTable("AssetModels");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.ConfigureAuditEntity();

        // Properties Configuration
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Code)
            .HasConversion<NaturalKeyConverter>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.ParentId)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.AssetCategoryId)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.AssetTypeId)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.ThumbnailFileId)
            .ValueGeneratedNever(); // We generate GUID v7 manually

        builder.Property(x => x.ThumbnailUrl);

        // Relationships
        builder.HasOne<AssetCategory>()
            .WithMany()
            .HasForeignKey(x => x.AssetCategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<AssetType>()
            .WithMany()
            .HasForeignKey(x => x.AssetTypeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Self-referencing relationship for Parent
        builder.HasOne<AssetModel>()
            .WithMany()
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Parameters)
            .WithOne()
            .HasForeignKey(x => x.AssetModelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Parameters)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.MaintenancePlanDefinitions)
            .WithOne()
            .HasForeignKey(x => x.AssetModelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.MaintenancePlanDefinitions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Images)
            .WithOne()
            .HasForeignKey(x => x.AssetModelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Images)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.ConfigureAuditEntity();

        // Indexes
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_AssetModels_Code");

        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_AssetModels_Name");

        builder.HasIndex(x => x.AssetCategoryId)
            .HasDatabaseName("IX_AssetModels_AssetCategoryId");

        builder.HasIndex(x => x.AssetTypeId)
            .HasDatabaseName("IX_AssetModels_AssetTypeId");

        builder.HasIndex(x => x.ParentId)
            .HasDatabaseName("IX_AssetModels_ParentId");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_AssetModels_IsActive");
    }
}

