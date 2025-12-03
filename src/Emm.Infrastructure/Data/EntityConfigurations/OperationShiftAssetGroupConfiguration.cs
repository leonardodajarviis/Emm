using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class OperationShiftAssetGroupConfiguration : IEntityTypeConfiguration<OperationShiftAssetGroup>
{
    public void Configure(EntityTypeBuilder<OperationShiftAssetGroup> builder)
    {
        builder.ToTable("OperationShiftAssetGroups");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.LinkedId)
            .IsRequired();

        builder.Property(x => x.OperationShiftId)
            .IsRequired();

        builder.Property(x => x.GroupName)
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

        builder.HasIndex(x => x.LinkedId)
            .IsUnique();

        builder.HasIndex(x => new { x.OperationShiftId, x.LinkedId })
            .IsUnique();

        builder.HasIndex(x => new { x.OperationShiftId, x.DisplayOrder });

        builder.HasIndex(x => x.Role);
    }
}
