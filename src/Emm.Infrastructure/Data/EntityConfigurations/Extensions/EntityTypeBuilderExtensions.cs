using Emm.Domain.Abstractions;
using Emm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emm.Infrastructure.Data.EntityConfigurations.Extensions;

public static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<TEntity> ConfigureAuditEntity<TEntity>(
        this EntityTypeBuilder<TEntity> builder)
        where TEntity : class, IAuditableEntity
    {
        // SỬ DỤNG OWNSONE THAY VÌ COMPLEXPROPERTY
        builder.OwnsOne(o => o.Audit, cb =>
        {
            // 1. Cấu hình cột
            cb.Property(a => a.CreatedByUserId)
              .HasColumnName("CreatedByUserId")
              .IsRequired();

            cb.Property(a => a.CreatedAt)
              .HasColumnName("CreatedAt")
              .IsRequired();

            cb.Property(a => a.ModifiedByUserId)
              .HasColumnName("ModifiedByUserId")
              .IsRequired(false);

            cb.Property(a => a.ModifiedAt)
              .HasColumnName("ModifiedAt")
              .IsRequired(false);

            // 2. Cấu hình Foreign Key NGAY TẠI ĐÂY
            // Lợi thế: Bạn truy cập trực tiếp 'a.CreatedByUserId', không cần 'e.Audit...'
            // nên không bị lỗi Expression nữa.

            // --- FK cho CreatedBy ---
            cb.HasOne<User>() // Entity User của bạn
              .WithMany()
              .HasForeignKey(a => a.CreatedByUserId) // Trỏ trực tiếp property trong VO
              .HasPrincipalKey(u => u.Id)
              .OnDelete(DeleteBehavior.Restrict);

            // --- FK cho ModifiedBy ---
            cb.HasOne<User>()
              .WithMany()
              .HasForeignKey(a => a.ModifiedByUserId)
              .HasPrincipalKey(u => u.Id)
              .OnDelete(DeleteBehavior.Restrict);
        });

        // Cấu hình Navigation Property (Optional)
        // Nếu Entity cha (TEntity) có property public User Creator { get; set; }
        // Bạn cần chỉ định EF biết nó map vào mối quan hệ trong Owned Type
        builder.Navigation(x => x.Audit).IsRequired();

        return builder;
    }
}
