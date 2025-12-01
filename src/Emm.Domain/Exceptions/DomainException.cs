namespace Emm.Domain.Exceptions;

/// <summary>
/// Base domain exception - thuần túy không có error codes
/// Error codes là responsibility của Application layer
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Entity không tìm thấy
/// </summary>
public class EntityNotFoundException : DomainException
{
    public string EntityName { get; }
    public object? EntityId { get; }

    public EntityNotFoundException(string entityName, object? entityId = null)
        : base($"{entityName} not found" + (entityId != null ? $" with id: {entityId}" : ""))
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}

/// <summary>
/// Vi phạm business rule
/// </summary>
public class BusinessRuleViolationException : DomainException
{
    public string RuleName { get; }

    public BusinessRuleViolationException(string ruleName, string message)
        : base(message)
    {
        RuleName = ruleName;
    }
}

/// <summary>
/// Entity ở trạng thái không hợp lệ để thực hiện operation
/// </summary>
public class InvalidEntityStateException : DomainException
{
    public string EntityName { get; }
    public string CurrentState { get; }

    public InvalidEntityStateException(string entityName, string currentState, string message)
        : base(message)
    {
        EntityName = entityName;
        CurrentState = currentState;
    }
}

/// <summary>
/// Dữ liệu không hợp lệ (validation trong domain)
/// </summary>
public class InvalidValueException : DomainException
{
    public string PropertyName { get; }
    public object? InvalidValue { get; }

    public InvalidValueException(string propertyName, object? invalidValue, string message)
        : base(message)
    {
        PropertyName = propertyName;
        InvalidValue = invalidValue;
    }
}

/// <summary>
/// Entity đã tồn tại (duplicate)
/// </summary>
public class EntityAlreadyExistsException : DomainException
{
    public string EntityName { get; }
    public string? PropertyName { get; }
    public object? PropertyValue { get; }

    public EntityAlreadyExistsException(string entityName, string? propertyName = null, object? propertyValue = null)
        : base($"{entityName} already exists" +
               (propertyName != null ? $" with {propertyName}: {propertyValue}" : ""))
    {
        EntityName = entityName;
        PropertyName = propertyName;
        PropertyValue = propertyValue;
    }
}

/// <summary>
/// Không có quyền thực hiện operation
/// </summary>
public class UnauthorizedAccessException : DomainException
{
    public string? Resource { get; }
    public string? Action { get; }

    public UnauthorizedAccessException(string message, string? resource = null, string? action = null)
        : base(message)
    {
        Resource = resource;
        Action = action;
    }
}

/// <summary>
/// Entity đang được sử dụng, không thể xóa/sửa
/// </summary>
public class EntityInUseException : DomainException
{
    public string EntityName { get; }
    public string? DependentEntity { get; }

    public EntityInUseException(string entityName, string? dependentEntity = null)
        : base($"{entityName} is in use" +
               (dependentEntity != null ? $" by {dependentEntity}" : "") +
               " and cannot be deleted")
    {
        EntityName = entityName;
        DependentEntity = dependentEntity;
    }
}

