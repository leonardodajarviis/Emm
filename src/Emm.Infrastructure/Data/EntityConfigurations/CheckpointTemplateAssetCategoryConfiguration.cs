using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class CheckpointTemplateAssetCategoryConfiguration : IEntityTypeConfiguration<CheckpointTemplateAssetCategory>
{
    public void Configure(EntityTypeBuilder<CheckpointTemplateAssetCategory> builder)
    {
        builder.ToTable("CheckpointTemplateAssetCategories");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        // Properties
        builder.Property(x => x.CheckpointTemplateId)
            .IsRequired();

        builder.Property(x => x.AssetCategoryId)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.AssignedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.RemovedAt)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(x => x.CheckpointTemplateId)
            .HasDatabaseName("IX_CheckpointTemplateAssetCategories_TemplateId");

        builder.HasIndex(x => x.AssetCategoryId)
            .HasDatabaseName("IX_CheckpointTemplateAssetCategories_CategoryId");

        builder.HasIndex(x => new { x.CheckpointTemplateId, x.AssetCategoryId })
            .HasDatabaseName("IX_CheckpointTemplateAssetCategories_TemplateId_CategoryId");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_CheckpointTemplateAssetCategories_IsActive");

        builder.HasIndex(x => x.AssignedAt)
            .HasDatabaseName("IX_CheckpointTemplateAssetCategories_AssignedAt");

        // Foreign Keys
        builder.HasOne<CheckpointTemplate>()
            .WithMany()
            .HasForeignKey(x => x.CheckpointTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        // Note: AssetCategory FK will be added via FluentAPI or separate configuration
        // to avoid circular reference issues
    }
}
