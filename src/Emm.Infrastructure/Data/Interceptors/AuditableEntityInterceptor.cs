using Emm.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Emm.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor tự động set CreatedAt và UpdatedAt cho các entity kế thừa AuditableEntity
/// </summary>
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateAuditableEntities(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker
            .Entries<IAuditableEntity>();

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.SetCreatedAt(now);
                entry.Entity.SetUpdatedAt(now);
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.SetUpdatedAt(now);

                // Đảm bảo CreatedAt không bị thay đổi
                entry.Property(e => e.CreatedAt).IsModified = false;
            }
        }
    }
}
