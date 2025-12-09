using Emm.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.ToTable("UnitOfMeasures");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Symbol)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.UnitType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.BaseUnitId)
            .IsRequired(false);

        builder.Property(x => x.ConversionFactor)
            .HasPrecision(18, 6)
            .IsRequired(false);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasIndex(x => x.Symbol);

        builder.HasIndex(x => x.UnitType);

        builder.HasIndex(x => new { x.IsActive, x.UnitType });

        builder.HasMany(x => x.DerivedUnits)
            .WithOne()
            .HasForeignKey(x => x.BaseUnitId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(x => x.DerivedUnits)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
