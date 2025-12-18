using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class ParameterBasedMaintenanceTriggerConfiguration : IEntityTypeConfiguration<ParameterBasedMaintenanceTrigger>
{
    public void Configure(EntityTypeBuilder<ParameterBasedMaintenanceTrigger> builder)
    {
        builder.ToTable("ParameterBasedMaintenanceTriggers");

        // Primary Key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        // Foreign Keys
        builder.Property(x => x.MaintenancePlanDefinitionId)
            .IsRequired();

        builder.Property(x => x.ParameterId)
            .IsRequired();

        // Required Properties
        builder.Property(x => x.TriggerValue)
            .IsRequired()
            .HasColumnType("decimal(18,4)");

        builder.Property(x => x.MinValue)
            .IsRequired()
            .HasColumnType("decimal(18,4)");

        builder.Property(x => x.MaxValue)
            .IsRequired()
            .HasColumnType("decimal(18,4)");

        builder.Property(x => x.TriggerCondition)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Note: ParameterCatalog relationship would be configured if needed
        // builder.HasOne<ParameterCatalog>()
        //     .WithMany()
        //     .HasForeignKey(x => x.ParameterId)
        //     .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.MaintenancePlanDefinitionId);
        builder.HasIndex(x => x.ParameterId);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.TriggerCondition);
        builder.HasIndex(x => new { x.ParameterId, x.IsActive });

        // Composite index for efficient trigger checking
        builder.HasIndex(x => new { x.MaintenancePlanDefinitionId, x.ParameterId, x.IsActive });
    }
}
