using Emm.Domain.Entities.AssetTransaction;
using Emm.Domain.Entities.Organization;
using Emm.Infrastructure.Data.Converters;
using Emm.Infrastructure.Data.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class AssetAdditionConfiguration : IEntityTypeConfiguration<AssetAddition>
{
    public void Configure(EntityTypeBuilder<AssetAddition> builder)
    {
        builder.ToTable("AssetAdditions");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties Configuration
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Code)
            .HasConversion<NaturalKeyConverter>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.OrganizationUnitId)
            .IsRequired();

        builder.Property(x => x.LocationId)
            .IsRequired();

        builder.Property(x => x.DecisionNumber)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.DecisionDate)
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.Property(x => x.Reason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.ConfigureAuditEntity();

        // Indexes
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_AssetAdditions_Code");

        builder.HasIndex(x => x.OrganizationUnitId)
            .HasDatabaseName("IX_AssetAdditions_OrganizationUnitId");

        builder.HasIndex(x => x.LocationId)
            .HasDatabaseName("IX_AssetAdditions_LocationId");

        // Relationships
        builder.HasMany<AssetAdditionLine>("_assetAdditionLines")
            .WithOne(e => e.AssetAddition)
            .HasForeignKey(e => e.AssetAdditionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<OrganizationUnit>()
            .WithMany()
            .HasForeignKey(e => e.OrganizationUnitId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Location>()
            .WithMany()
            .HasForeignKey(e => e.LocationId)
            .OnDelete(DeleteBehavior.NoAction);


        builder.Ignore(x => x.AssetAdditionLines);
    }
}
