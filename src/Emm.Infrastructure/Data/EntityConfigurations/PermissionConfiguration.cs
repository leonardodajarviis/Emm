using Emm.Domain.Entities.Authorization;
using Emm.Infrastructure.Data.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Resource)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.Category)
            .HasMaxLength(100);

        builder.ConfigureAuditEntity();

        // Indexes
        builder.HasIndex(p => p.Code).IsUnique();
        builder.HasIndex(p => p.Resource);
        builder.HasIndex(p => p.Category);
    }
}
