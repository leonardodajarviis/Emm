using Emm.Domain.Entities;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
