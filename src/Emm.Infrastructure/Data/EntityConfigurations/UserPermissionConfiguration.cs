using Emm.Domain.Entities;
using Emm.Domain.Entities.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable("UserPermissions");

        // Composite key
        builder.HasKey(up => new { up.UserId, up.PermissionId });

        builder.Property(up => up.IsGranted)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(up => up.AssignedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(up => up.AssignedBy)
            .IsRequired(false);

        builder.Property(up => up.Reason)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(up => up.User)
            .WithMany()
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(up => up.Permission)
            .WithMany(p => p.UserPermissions)
            .HasForeignKey(up => up.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(up => up.UserId);
        builder.HasIndex(up => up.PermissionId);
    }
}
