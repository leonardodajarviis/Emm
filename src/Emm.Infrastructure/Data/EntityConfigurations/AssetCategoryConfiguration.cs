using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Infrastructure.Data.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class AssetCategoryConfiguration : IEntityTypeConfiguration<AssetCategory>
{
    public void Configure(EntityTypeBuilder<AssetCategory> builder)
    {
        builder.ToTable("AssetCategories");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties Configuration
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
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

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.ConfigureAuditEntity();

        // Indexes
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_AssetCategories_Code");

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_AssetCategories_Name");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_AssetCategories_IsActive");
    }
}
