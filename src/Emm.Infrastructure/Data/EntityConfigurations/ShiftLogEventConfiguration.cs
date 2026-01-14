using Emm.Domain.Entities.Maintenance;
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
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.ShiftLogId)
            .IsRequired();

        builder.Property(x => x.EventType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.IncidentId)
            .IsRequired(false);

        builder.Property(x => x.StartTime)
            .IsRequired();

        builder.Property(x => x.EndTime).IsRequired();

        builder.Property(x => x.Duration).IsRequired();

        builder.HasOne<IncidentReport>()
            .WithMany()
            .HasForeignKey(x => x.IncidentId)
            .OnDelete(DeleteBehavior.NoAction);

        // Indexes
        builder.HasIndex(x => x.ShiftLogId);

        builder.HasIndex(x => x.EventType);

        builder.HasIndex(x => x.StartTime);

        builder.HasIndex(x => new { x.ShiftLogId, x.EventType });
    }
}
