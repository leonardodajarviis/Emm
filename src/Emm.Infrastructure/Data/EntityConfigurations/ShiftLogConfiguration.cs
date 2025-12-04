using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class ShiftLogConfiguration : IEntityTypeConfiguration<ShiftLog>
{
    public void Configure(EntityTypeBuilder<ShiftLog> builder)
    {
        builder.ToTable("ShiftLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.OperationShiftId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.StartTime);

        builder.Property(x => x.EndTime);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        // Asset and Group references
        builder.Property(x => x.AssetId);

        builder.Property(x => x.GroupId);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Foreign key reference to OperationShift (separate aggregate)
        builder.HasIndex(x => x.OperationShiftId);

        // Indexes for AssetId and GroupId
        builder.HasIndex(x => x.AssetId);
        builder.HasIndex(x => x.GroupId);

        // Collections - using backing field pattern

        builder.HasMany(x => x.Readings)
            .WithOne()
            .HasForeignKey(x => x.ShiftLogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Readings)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Checkpoints)
            .WithOne()
            .HasForeignKey(x => x.ShiftLogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Checkpoints)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Events)
            .WithOne()
            .HasForeignKey(x => x.ShiftLogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Events)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.ShiftLogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
