using Emm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.Configurations;

public class UploadedFileConfiguration : IEntityTypeConfiguration<UploadedFile>
{
    public void Configure(EntityTypeBuilder<UploadedFile> builder)
    {
        builder.ToTable("UploadedFiles");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever(); // We generate GUID v7 manually

        builder.Property(f => f.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(f => f.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(f => f.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(f => f.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.FileSize)
            .IsRequired();

        builder.Property(f => f.Subfolder)
            .HasMaxLength(255);

        builder.Property(f => f.UploadedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.HasIndex(f => f.Subfolder);

        builder.HasIndex(f => f.UploadedAt);
    }
}
