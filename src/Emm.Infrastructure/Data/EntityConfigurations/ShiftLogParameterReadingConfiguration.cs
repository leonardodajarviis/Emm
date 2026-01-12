using Emm.Domain.Entities;
using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class ShiftLogParameterReadingConfiguration : IEntityTypeConfiguration<ShiftLogParameterReading>
{
    public void Configure(EntityTypeBuilder<ShiftLogParameterReading> builder)
    {
        builder.ToTable("ShiftLogParameterReadings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();


        builder.Property(x => x.ShiftLogId)
            .IsRequired();

        builder.Property(x => x.OperationShiftId)
            .IsRequired();

        builder.Property(x => x.AssetId)
            .IsRequired();

        builder.Property(x => x.AssetCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.AssetName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ParameterId)
            .IsRequired();

        builder.Property(x => x.ParameterName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ParameterCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Value)
            .IsRequired()
            .HasColumnType("decimal(18,6)");

        builder.Property(x => x.StringValue)
            .HasMaxLength(500);

        builder.Property(x => x.ReadingTime)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasOne<UnitOfMeasure>()
            .WithMany()
            .HasForeignKey(x => x.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<OperationShift>()
            .WithMany()
            .HasForeignKey(x => x.OperationShiftId)
            .OnDelete(DeleteBehavior.NoAction);

        // Indexes
        builder.HasIndex(x => x.ShiftLogId);

        builder.HasIndex(x => x.AssetId);

        builder.HasIndex(x => x.ParameterId);

        builder.HasIndex(x => x.ParameterCode);

        builder.HasIndex(x => x.ReadingTime);


        // Composite indexes for queries
        builder.HasIndex(x => new { x.AssetId, x.ParameterId, x.ReadingTime });

        builder.HasIndex(x => new { x.ShiftLogId, x.AssetId });
    }
}
