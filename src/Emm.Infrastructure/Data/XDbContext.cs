using Emm.Application.Abstractions;
using Emm.Domain.Abstractions;
using Emm.Domain.Entities;
using Emm.Domain.ValueObjects;
using Emm.Infrastructure.Data.Interceptors;
using Emm.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Data;

public class XDbContext : DbContext
{
    private readonly DomainEventInterceptor _domainEventInterceptor;
    private readonly IDateTimeProvider _clock;
    private readonly IUserContextService _userContext;

    public XDbContext(
        DbContextOptions<XDbContext> options,
        IUserContextService userContextService,
        IDateTimeProvider dateTimeProvider,
        DomainEventInterceptor domainEventInterceptor) : base(options)
    {
        _domainEventInterceptor = domainEventInterceptor;
        _userContext = userContextService;
        _clock = dateTimeProvider;
    }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<SequenceNumber> SequenceNumbers { get; set; }
    public DbSet<UploadedFile> UploadedFiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_domainEventInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Owned<AuditMetadata>();
        // modelBuilder.Owned<NaturalKey>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(XDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        ApplyAudit();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAudit();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAudit()
    {
        var entries = ChangeTracker
            .Entries<IAuditableEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .ToList();

        var now = _clock.Now;
        var systemUserId = Guid.Parse("019B46BE-4876-7824-BBC0-468325FB38C1");
        var userId = _userContext.GetCurrentUserId() ?? systemUserId;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetAudit(
                        AuditMetadata.Create(userId, now));
                    break;

                case EntityState.Modified:
                    if (!entry.Properties.Any(p => p.IsModified))
                        continue;

                    var audit = entry.Entity.Audit is null
                        ? AuditMetadata.Create(userId, now)
                        : entry.Entity.Audit.Update(userId, now);

                    entry.Entity.SetAudit(audit);
                    break;
            }
        }
    }
}
