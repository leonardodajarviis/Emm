using Emm.Domain.Entities.AssetCatalog;
using Emm.Infrastructure.Data.EntityConfigurations.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class MaintenancePlanDefinitionConfiguration : IEntityTypeConfiguration<MaintenancePlanDefinition>
{
    public void Configure(EntityTypeBuilder<MaintenancePlanDefinition> builder)
    {
        builder.ToTable("MaintenancePlanDefinitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.AssetModelId)
            .IsRequired();

        builder.Property(x => x.PlanType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.RRule);

        builder.ConfigureAuditEntity();

        builder.HasIndex(x => new { x.AssetModelId, x.IsActive, x.PlanType })
            .HasDatabaseName("IX_MaintenancePlanDefinitions_AssetModelId_IsActive_PlanType");

        // Navigation properties
        builder.HasOne(x => x.ParameterBasedTrigger)
            .WithOne()
            .HasForeignKey<ParameterBasedMaintenanceTrigger>(x => x.MaintenancePlanDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.ParameterBasedTrigger)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.JobSteps)
            .WithOne()
            .HasForeignKey(x => x.MaintenancePlanDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.JobSteps)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
