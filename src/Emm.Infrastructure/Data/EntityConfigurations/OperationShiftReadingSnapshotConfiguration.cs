using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class OperationShiftReadingSnapshotConfiguration : IEntityTypeConfiguration<OperationShiftReadingSnapshot>
{
    public void Configure(EntityTypeBuilder<OperationShiftReadingSnapshot> builder)
    {
        builder.ToTable("OperationShiftReadingSnapshots");

        builder.HasKey(x => new { x.OperationShiftId, x.AssetId, x.ParameterId });

        builder.Property(x => x.OperationShiftId)
            .IsRequired();

        builder.Property(x => x.AssetId)
            .IsRequired();

        builder.Property(x => x.ParameterId)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired()
            .HasColumnType("decimal(18,6)");
    }
}
