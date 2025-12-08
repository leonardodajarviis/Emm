namespace Emm.Domain.Abstractions;

/// <summary>
/// Interface cho các entity cần tự động quản lý CreatedAt và UpdatedAt.
/// Entity implement interface này sẽ tự động được set timestamps bởi AuditableEntityInterceptor.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Thời điểm tạo entity (UTC)
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Thời điểm cập nhật gần nhất (UTC)
    /// </summary>
    DateTime UpdatedAt { get; }

    long? CreatedByUserId { get; }
    long? UpdatedByUserId { get; }
}

/// <summary>
/// Extension methods để set audit timestamps từ Interceptor.
/// Sử dụng reflection để có thể set private setters.
/// </summary>
public static class AuditableEntityExtensions
{
    /// <summary>
    /// Public method cho Interceptor sử dụng để set CreatedAt.
    /// Sử dụng reflection để bypass private setter.
    /// </summary>
    public static void SetCreatedAt(this IAuditableEntity entity, DateTime createdAt)
    {
        var property = entity.GetType().GetProperty(nameof(IAuditableEntity.CreatedAt));
        property?.SetValue(entity, createdAt);
    }

    /// <summary>
    /// Public method cho Interceptor sử dụng để set UpdatedAt.
    /// Sử dụng reflection để bypass private setter.
    /// </summary>
    public static void SetUpdatedAt(this IAuditableEntity entity, DateTime updatedAt)
    {
        var property = entity.GetType().GetProperty(nameof(IAuditableEntity.UpdatedAt));
        property?.SetValue(entity, updatedAt);
    }
}
