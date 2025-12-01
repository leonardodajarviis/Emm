using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations;


public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.DisplayName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .HasMaxLength(100);

        builder.Property(e => e.OrganizationUnitId)
            .IsRequired(false);

        builder.HasOne<OrganizationUnit>()
            .WithMany()
            .HasForeignKey(e => e.OrganizationUnitId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(e => e.Code)
            .IsUnique()
            .HasDatabaseName("IX_Employees_Code");
    }
}