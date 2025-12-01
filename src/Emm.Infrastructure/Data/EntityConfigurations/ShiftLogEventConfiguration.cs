using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class ShiftLogEventConfiguration : IEntityTypeConfiguration<ShiftLogEvent>
{
    public void Configure(EntityTypeBuilder<ShiftLogEvent> builder)
    {
        builder.ToTable("ShiftLogEvents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.ShiftLogId)
            .IsRequired();

        builder.Property(x => x.EventType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.StartTime)
            .IsRequired();

        builder.Property(x => x.EndTime);

        builder.Property(x => x.Duration);

        // Indexes
        builder.HasIndex(x => x.ShiftLogId);

        builder.HasIndex(x => x.EventType);


        builder.HasIndex(x => x.StartTime);

        builder.HasIndex(x => new { x.ShiftLogId, x.EventType });
    }
}
