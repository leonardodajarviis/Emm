using Emm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class UnitOfMeasureCategoryConfiguration : IEntityTypeConfiguration<UnitOfMeasureCategory>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasureCategory> builder)
    {
        builder.ToTable("UnitOfMeasureCategories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(20);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.UnitOfMeasureCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Lines)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
