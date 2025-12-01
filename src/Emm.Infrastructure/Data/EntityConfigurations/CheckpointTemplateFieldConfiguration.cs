using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;

public class CheckpointTemplateFieldConfiguration : IEntityTypeConfiguration<CheckpointTemplateField>
{
    public void Configure(EntityTypeBuilder<CheckpointTemplateField> builder)
    {
        builder.ToTable("CheckpointTemplateFields");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        // Properties
        builder.Property(x => x.CheckpointTemplateId)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.FieldType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.IsRequired)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.Order)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.DefaultValue)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.ValidationRules)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.Property(x => x.MasterDataTypeId)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(x => x.CheckpointTemplateId)
            .HasDatabaseName("IX_CheckpointTemplateFields_TemplateId");

        builder.HasIndex(x => new { x.CheckpointTemplateId, x.Code })
            .IsUnique()
            .HasDatabaseName("IX_CheckpointTemplateFields_TemplateId_Code");

        builder.HasIndex(x => x.FieldType)
            .HasDatabaseName("IX_CheckpointTemplateFields_FieldType");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_CheckpointTemplateFields_IsActive");

        builder.HasIndex(x => x.Order)
            .HasDatabaseName("IX_CheckpointTemplateFields_Order");

        builder.HasIndex(x => x.MasterDataTypeId)
            .HasDatabaseName("IX_CheckpointTemplateFields_MasterDataTypeId");
    }
}
