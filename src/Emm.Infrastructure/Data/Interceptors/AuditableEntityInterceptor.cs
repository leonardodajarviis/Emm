using Emm.Application.Abstractions;
using Emm.Domain.Abstractions;
using Emm.Domain.Entities;
using Emm.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Emm.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor tự động set CreatedAt và UpdatedAt cho các entity kế thừa AuditableEntity
/// </summary>
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUserContextService _userContextService;

    public AuditableEntityInterceptor(IDateTimeProvider dateTimeProvider, IUserContextService userContextService)
    {
        _dateTimeProvider = dateTimeProvider;
        _userContextService = userContextService;
    }
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

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker
            .Entries<IAuditableEntity>();

        var userId = _userContextService.GetCurrentUserId();
        if (userId is null)
        {
            userId = 1;
        }

        var now = _dateTimeProvider.Now;

        foreach (var entry in entries)
        {
            // Chỉ xử lý các entity đã có AuditMetadata
            if (entry.Entity.Audit is null)
            {
                continue; // Skip nếu entity chưa có Audit
            }

            if (entry.State == EntityState.Added)
            {
                entry.Entity.SetAudit(AuditMetadata.Create(userId.Value, now));
            }
            else if (entry.State == EntityState.Modified)
            {
                var currentAudit = entry.Entity.Audit;
                entry.Entity.SetAudit(currentAudit.Update(userId.Value, now));
            }
        }
    }
}
