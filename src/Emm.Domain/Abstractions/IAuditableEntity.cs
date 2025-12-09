using Emm.Domain.ValueObjects;

namespace Emm.Domain.Abstractions;

/// <summary>
/// Interface cho các entity cần tự động quản lý CreatedAt và UpdatedAt.
/// Entity implement interface này sẽ tự động được set timestamps bởi AuditableEntityInterceptor.
/// </summary>
public interface IAuditableEntity
{
    AuditMetadata Audit {get;}
    void SetAudit(AuditMetadata audit);
}
