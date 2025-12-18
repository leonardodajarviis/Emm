using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;


public class OrganizationUnitLevelConfiguration : IEntityTypeConfiguration<OrganizationUnitLevel>
{
    public void Configure(EntityTypeBuilder<OrganizationUnitLevel> builder)
    {
        builder.ToTable("OrganizationUnitLevels");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties Configuration
        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Level)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_OrganizationUnitLevels_Name");
    }
}