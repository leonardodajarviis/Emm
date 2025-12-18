using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class ShiftLogItemConfiguration : IEntityTypeConfiguration<ShiftLogItem>
{
    public void Configure(EntityTypeBuilder<ShiftLogItem> builder)
    {
        builder.ToTable("ShiftLogItems");

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ShiftLogId)
            .IsRequired();

        builder.Property(x => x.ItemId)
            .IsRequired();

        builder.Property(x => x.ItemName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(x => x.AssetId);

        builder.Property(x => x.AssetCode)
            .HasMaxLength(100);

        builder.Property(x => x.AssetName)
            .HasMaxLength(200);

        builder.Property(x => x.UnitOfMeasureId);

        builder.Property(x => x.UnitOfMeasureName)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(x => x.ShiftLogId);
        builder.HasIndex(x => x.ItemId);
    }
}
