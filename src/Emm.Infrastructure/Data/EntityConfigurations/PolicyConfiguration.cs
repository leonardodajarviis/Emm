using Emm.Domain.Entities.Authorization;
using Emm.Infrastructure.Data.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.ToTable("Policies");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.ResourceType)
            .HasMaxLength(100);

        builder.Property(p => p.Action)
            .HasMaxLength(50);

        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.ConditionsJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)")
            .HasDefaultValue("{}");

        builder.Property(p => p.Priority)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.ConfigureAuditEntity();
        // Indexes
        builder.HasIndex(p => p.Code).IsUnique();
        builder.HasIndex(p => p.ResourceType);
        builder.HasIndex(p => new { p.IsActive, p.Priority });
    }
}
