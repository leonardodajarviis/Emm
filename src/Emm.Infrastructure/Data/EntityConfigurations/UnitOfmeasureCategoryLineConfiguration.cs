using Emm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class UnitOfmeasureCategoryLineConfiguration : IEntityTypeConfiguration<UnitOfMeasureCategoryLine>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasureCategoryLine> builder)
    {
        builder.ToTable("UnitOfMeasureCategoryLines");

        builder.HasKey(x => new { x.UnitOfMeasureCategoryId, x.UnitOfMeasureId });

        builder.HasOne<UnitOfMeasure>()
            .WithMany()
            .HasForeignKey(x => x.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
