using Emm.Domain.Entities;
using Emm.Infrastructure.Data.Interceptors;
using Emm.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Data;

public class XDbContext : DbContext
{
    private readonly AuditableEntityInterceptor _auditableInterceptor;

    public XDbContext(
        DbContextOptions<XDbContext> options,
        AuditableEntityInterceptor auditableInterceptor) : base(options)
    {
        _auditableInterceptor = auditableInterceptor;
    }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<SequenceNumber> SequenceNumbers { get; set; }
    public DbSet<UploadedFile> UploadedFiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(XDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
