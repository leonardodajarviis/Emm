using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class SequenceNumberConfiguration : IEntityTypeConfiguration<SequenceNumber>
{
    public void Configure(EntityTypeBuilder<SequenceNumber> builder)
    {
        builder.ToTable("SequenceNumbers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Prefix)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.TableName)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.NumberLength)
            .IsRequired();

        builder.Property(x => x.CurrentNumber)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Ensure single row per (Prefix, TableName, NumberLength)
        builder.HasIndex(x => new { x.Prefix, x.TableName, x.NumberLength })
            .IsUnique()
            .HasDatabaseName("UX_SequenceNumbers_Prefix_Table_NumberLength");
    }
}

