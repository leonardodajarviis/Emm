using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class OrganizationUnitConfiguration : IEntityTypeConfiguration<OrganizationUnit>
{
    public void Configure(EntityTypeBuilder<OrganizationUnit> builder)
    {
        builder.ToTable("OrganizationUnits");

        // Primary Key
        builder.HasKey(x => x.Id);
        
        // Properties Configuration
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        
        builder.HasOne<OrganizationUnitLevel>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationUnitLevelId)
            .OnDelete(DeleteBehavior.NoAction);

        // Indexes
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_OrganizationUnits_Code");

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_OrganizationUnits_Name");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_OrganizationUnits_IsActive");
    }
}