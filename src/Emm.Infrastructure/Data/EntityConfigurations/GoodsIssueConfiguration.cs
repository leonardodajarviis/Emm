using Emm.Domain.Entities.Inventory;
using Emm.Domain.ValueObjects;
using Emm.Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;


public class GoodsIssueConfiguration : IEntityTypeConfiguration<GoodsIssue>
{
    public void Configure(EntityTypeBuilder<GoodsIssue> builder)
    {
        builder.ToTable("GoodsIssues");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.Code)
            .HasConversion<NaturalKeyConverter>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasMaxLength(20)
            .HasConversion(
                v => v.Value,
                v => DocumentStatus.From(v)
            );

        // Additional property configurations can be added here as needed
    }
}
