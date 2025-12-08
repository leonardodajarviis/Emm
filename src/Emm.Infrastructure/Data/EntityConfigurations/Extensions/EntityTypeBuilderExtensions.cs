using Emm.Domain.Abstractions;
using Emm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations.Extensions;

public static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// Cấu hình các thuộc tính audit (CreatedAt, UpdatedAt, CreatedByUserId, UpdatedByUserId) cho entity
    /// </summary>
    public static EntityTypeBuilder<TEntity> ConfigureAuditInfo<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        bool withUserTracking = true)
        where TEntity : class, IAuditableEntity
    {
        // CreatedAt
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // UpdatedAt
        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        if (withUserTracking)
        {
            // CreatedByUserId
            builder.Property(x => x.CreatedByUserId)
                .IsRequired(false);

            // UpdatedByUserId
            builder.Property(x => x.UpdatedByUserId)
                .IsRequired(false);

            // Foreign Key Relationships
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        return builder;
    }
}
