using Emm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.AccessTokenJti)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.RefreshTokenJti)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.AccessTokenExpiresAt)
            .IsRequired();

        builder.Property(s => s.RefreshTokenExpiresAt)
            .IsRequired();

        builder.Property(s => s.IpAddress)
            .HasMaxLength(50);

        builder.Property(s => s.UserAgent)
            .HasMaxLength(500);

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(s => s.RevokedAt)
            .IsRequired(false);

        // Indexes for performance
        builder.HasIndex(s => s.RefreshTokenJti)
            .HasDatabaseName("IX_UserSessions_RefreshTokenJti");

        builder.HasIndex(s => s.AccessTokenJti)
            .HasDatabaseName("IX_UserSessions_AccessTokenJti");

        builder.HasIndex(s => new { s.UserId, s.RevokedAt })
            .HasDatabaseName("IX_UserSessions_UserId_RevokedAt");

        // Foreign key relationship
        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
