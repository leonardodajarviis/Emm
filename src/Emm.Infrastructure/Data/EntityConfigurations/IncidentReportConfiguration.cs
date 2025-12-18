using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Maintenance;
using Emm.Infrastructure.Data.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class IncidentReportConfiguration : IEntityTypeConfiguration<IncidentReport>
{
    public void Configure(EntityTypeBuilder<IncidentReport> builder)
    {
        builder.ToTable("IncidentReports");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.ResolutionNotes)
            .IsRequired(false);

        builder.Property(x => x.ReportedAt)
            .IsRequired();

        builder.Property(x => x.ResolvedAt)
            .IsRequired(false);

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.ConfigureAuditEntity();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasOne<Asset>()
            .WithMany()
            .HasForeignKey(x => x.AssetId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
