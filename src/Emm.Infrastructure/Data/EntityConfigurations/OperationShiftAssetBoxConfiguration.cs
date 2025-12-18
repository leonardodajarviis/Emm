using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class OperationShiftAssetBoxConfiguration : IEntityTypeConfiguration<OperationShiftAssetBox>
{
    public void Configure(EntityTypeBuilder<OperationShiftAssetBox> builder)
    {
        builder.ToTable("OperationShiftAssetBoxes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.OperationShiftId)
            .IsRequired();

        builder.Property(x => x.BoxName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.Role)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.OperationShiftId);

        builder.HasIndex(x => new { x.OperationShiftId, x.DisplayOrder });

        builder.HasIndex(x => x.Role);

        // Unique constraint: BoxName must be unique within an OperationShift
        builder.HasIndex(x => new { x.OperationShiftId, x.BoxName })
            .IsUnique()
            .HasDatabaseName("IX_OperationShiftAssetBoxes_OperationShiftId_BoxName_Unique");
    }
}
