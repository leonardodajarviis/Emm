using Emm.Domain.Entities;
using Emm.Infrastructure.Data.Interceptors;
using Emm.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Data;

public class XDbContext : DbContext
{
    private readonly AuditableEntityInterceptor _auditableInterceptor;
    private readonly DomainEventInterceptor _domainEventInterceptor;

    public XDbContext(
        DbContextOptions<XDbContext> options,
        AuditableEntityInterceptor auditableInterceptor,
        DomainEventInterceptor domainEventInterceptor) : base(options)
    {
        _auditableInterceptor = auditableInterceptor;
        _domainEventInterceptor = domainEventInterceptor;
    }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<SequenceNumber> SequenceNumbers { get; set; }
    public DbSet<UploadedFile> UploadedFiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableInterceptor, _domainEventInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(XDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
