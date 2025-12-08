using Emm.Domain.Entities;
using Emm.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;


public class ParameterCatalogConfiguration : IEntityTypeConfiguration<ParameterCatalog>
{
    public void Configure(EntityTypeBuilder<ParameterCatalog> builder)
    {
        builder.ToTable("ParameterCatalogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.IsCodeGenerated)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValueSql("0");

        builder.Property(x => x.UnitOfMeasureId).IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.CreatedByUserId);

        builder.Property(x => x.UpdatedByUserId);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasOne<UnitOfMeasure>()
            .WithMany()
            .HasForeignKey(pc => pc.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(pc => pc.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(pc => pc.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
