using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class CheckpointTemplateConfiguration : IEntityTypeConfiguration<CheckpointTemplate>
{
    public void Configure(EntityTypeBuilder<CheckpointTemplate> builder)
    {
        builder.ToTable("CheckpointTemplates");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        // Properties
        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.Version)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationships - Use backing fields
        builder.HasMany(x => x.Fields)
            .WithOne()
            .HasForeignKey(f => f.CheckpointTemplateId)
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.PrincipalToDependent!.SetField("_fields");

        builder.HasMany(x => x.AssetCategories)
            .WithOne()
            .HasForeignKey(ac => ac.CheckpointTemplateId)
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.PrincipalToDependent!.SetField("_assetCategories");

        // Indexes
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_CheckpointTemplates_Code");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_CheckpointTemplates_IsActive");

        builder.HasIndex(x => x.Version)
            .HasDatabaseName("IX_CheckpointTemplates_Version");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_CheckpointTemplates_CreatedAt");

        // Ignore navigation properties from AggregateRoot
        builder.Ignore(x => x.DomainEvents);
    }
}
