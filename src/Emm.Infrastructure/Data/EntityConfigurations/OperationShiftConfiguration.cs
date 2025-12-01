using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class OperationShiftConfiguration : IEntityTypeConfiguration<OperationShift>
{
    public void Configure(EntityTypeBuilder<OperationShift> builder)
    {
        builder.ToTable("OperationShifts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(500);


        builder.Property(x => x.OrganizationUnitId)
            .IsRequired();

        builder.Property(x => x.PrimaryUserId)
            .IsRequired();

        builder.Property(x => x.ScheduledStartTime)
            .IsRequired();

        builder.Property(x => x.ScheduledEndTime)
            .IsRequired();

        builder.Property(x => x.ActualStartTime);

        builder.Property(x => x.ActualEndTime);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.IsCheckpointLogEnabled)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Collections - using backing field pattern
        builder.HasMany(x => x.Assets)
            .WithOne()
            .HasForeignKey(x => x.OperationShiftId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Assets)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Indexes
        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasIndex(x => x.OrganizationUnitId);

        builder.HasIndex(x => x.PrimaryUserId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.ScheduledStartTime);

        builder.HasIndex(x => x.CreatedAt);
    }
}
