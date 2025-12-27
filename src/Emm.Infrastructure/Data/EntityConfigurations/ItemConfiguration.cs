using Emm.Domain.Entities;
using Emm.Domain.Entities.Inventory;
using Emm.Infrastructure.Data.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.UnitOfMeasureId)
            .IsRequired();

        builder.ConfigureAuditEntity();

        builder.HasIndex(i => i.Code)
            .IsUnique();

        builder.HasOne<UnitOfMeasure>()
            .WithMany()
            .HasForeignKey(i => i.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<ItemGroup>()
            .WithMany()
            .HasForeignKey(i => i.GroupId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
