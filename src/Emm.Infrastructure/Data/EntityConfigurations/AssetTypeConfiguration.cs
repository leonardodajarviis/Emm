using Emm.Domain.Entities.AssetCatalog;
using Emm.Infrastructure.Data.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class AssetTypeConfiguration : IEntityTypeConfiguration<AssetType>
{
    public void Configure(EntityTypeBuilder<AssetType> builder)
    {
        builder.ToTable("AssetTypes");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties Configuration
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.AssetCategoryId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.ConfigureAuditEntity();

        // Relationships
        builder.HasOne<AssetCategory>()
            .WithMany()
            .HasForeignKey(x => x.AssetCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => new { x.Name, x.AssetCategoryId })
            .IsUnique()
            .HasDatabaseName("IX_AssetTypes_Name_AssetCategoryId");

        builder.HasIndex(x => x.AssetCategoryId)
            .HasDatabaseName("IX_AssetTypes_AssetCategoryId");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_AssetTypes_IsActive");

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_AssetTypes_Code");

        builder.HasMany<AssetTypeParameter>("_parameters")
            .WithOne()
            .HasForeignKey(e => e.AssetTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(x => x.Parameters);
        builder.Ignore(x => x.DomainEvents);
    }
}
