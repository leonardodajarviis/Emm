using Emm.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.Payload)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.ProcessedAt)
            .IsRequired(false);

        builder.Property(x => x.Error)
            .IsRequired(false);

        builder.Property(x => x.Attempt)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.LockId)
            .IsRequired(false)
            .HasMaxLength(128);

        builder.Property(x => x.LockedUntil)
            .IsRequired(false);

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsRequired();

        // Indexes for efficient querying
        builder.HasIndex(x => new { x.ProcessedAt, x.LockedUntil, x.CreatedAt })
            .HasDatabaseName("IX_OutboxMessages_Processing");

        builder.HasIndex(x => x.LockId)
            .HasDatabaseName("IX_OutboxMessages_LockId");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_OutboxMessages_CreatedAt");
    }
}
