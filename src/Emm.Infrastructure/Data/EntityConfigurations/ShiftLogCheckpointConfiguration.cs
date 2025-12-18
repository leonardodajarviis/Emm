using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class ShiftLogCheckpointConfiguration : IEntityTypeConfiguration<ShiftLogCheckpoint>
{
    public void Configure(EntityTypeBuilder<ShiftLogCheckpoint> builder)
    {
        builder.ToTable("ShiftLogCheckpoints");

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ShiftLogId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.LocationId)
            .IsRequired();

        builder.Property(x => x.LocationName)
            .IsRequired()
            .HasMaxLength(1200);

        builder.Property(x => x.IsWithAttachedMaterial)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.ShiftLogId);
    }
}

