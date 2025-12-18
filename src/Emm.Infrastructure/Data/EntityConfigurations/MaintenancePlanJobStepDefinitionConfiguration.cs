using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Organization;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;


public class MaintenancePlanJobStepDefinitionConfiguration : IEntityTypeConfiguration<MaintenancePlanJobStepDefinition>
{
    public void Configure(EntityTypeBuilder<MaintenancePlanJobStepDefinition> builder)
    {
        builder.ToTable("MaintenancePlanJobStepDefinitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.MaintenancePlanDefinitionId)
            .IsRequired();

        builder.Property(x => x.Note)
            .HasMaxLength(500);

        builder.Property(x => x.Order)
            .IsRequired();

        builder.Property(x => x.OrganizationUnitId)
            .IsRequired(false);

        builder.HasOne<OrganizationUnit>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationUnitId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.MaintenancePlanDefinitionId, x.Order })
            .HasDatabaseName("IX_MaintenancePlanJobStepDefinitions_MaintenancePlanDefinitionId_Order");
    }
}
