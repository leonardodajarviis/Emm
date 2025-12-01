namespace Emm.Domain.Exceptions;

/// <summary>
/// Guard clauses helper for Domain validation.
/// Giúp validate và throw appropriate domain exceptions.
/// </summary>
public static class DomainGuard
{
    #region Null/Empty Guards

    public static void AgainstNullOrEmpty(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidValueException(propertyName, value, $"{propertyName} is required");
        }
    }

    public static void AgainstNull<T>(T? value, string propertyName) where T : class
    {
        if (value is null)
        {
            throw new InvalidValueException(propertyName, null, $"{propertyName} is required");
        }
    }

    public static void AgainstDefault<T>(T value, string propertyName) where T : struct
    {
        if (value.Equals(default(T)))
        {
            throw new InvalidValueException(propertyName, value, $"{propertyName} is required");
        }
    }

    #endregion

    #region Number Guards

    public static void AgainstNegative(int value, string propertyName)
    {
        if (value < 0)
        {
            throw new InvalidValueException(propertyName, value, $"{propertyName} cannot be negative");
        }
    }

    public static void AgainstNegativeOrZero(int value, string propertyName)
    {
        if (value <= 0)
        {
            throw new InvalidValueException(propertyName, value, $"{propertyName} must be greater than zero");
        }
    }

    public static void AgainstNegative(long value, string propertyName)
    {
        if (value < 0)
        {
            throw new InvalidValueException(propertyName, value, $"{propertyName} cannot be negative");
        }
    }

    public static void AgainstNegativeOrZero(long value, string propertyName)
    {
        if (value <= 0)
        {
            throw new InvalidValueException(propertyName, value, $"{propertyName} must be greater than zero");
        }
    }

    public static void AgainstNegative(decimal value, string propertyName)
    {
        if (value < 0)
        {
            throw new InvalidValueException(propertyName, value, $"{propertyName} cannot be negative");
        }
    }

    public static void AgainstOutOfRange(int value, int min, int max, string propertyName)
    {
        if (value < min || value > max)
        {
            throw new InvalidValueException(propertyName, value, $"{propertyName} must be between {min} and {max}");
        }
    }

    #endregion

    #region String Length Guards

    public static void AgainstTooLong(string? value, int maxLength, string propertyName)
    {
        if (value?.Length > maxLength)
        {
            throw new InvalidValueException(propertyName, value, $"{propertyName} cannot exceed {maxLength} characters");
        }
    }

    public static void AgainstTooShort(string? value, int minLength, string propertyName)
    {
        if (value?.Length < minLength)
        {
            throw new InvalidValueException(propertyName, value, $"{propertyName} must be at least {minLength} characters");
        }
    }

    #endregion

    #region State Guards

    public static void AgainstInvalidState(bool condition, string entityName, string currentState, string message)
    {
        if (condition)
        {
            throw new InvalidEntityStateException(entityName, currentState, message);
        }
    }

    public static void AgainstBusinessRule(bool condition, string ruleName, string message)
    {
        if (condition)
        {
            throw new BusinessRuleViolationException(ruleName, message);
        }
    }

    #endregion

    #region Entity Guards

    public static void AgainstNotFound<T>(T? entity, string entityName, object? entityId = null) where T : class
    {
        if (entity is null)
        {
            throw new EntityNotFoundException(entityName, entityId);
        }
    }

    public static void AgainstDuplicate(bool exists, string entityName, string? propertyName = null, object? propertyValue = null)
    {
        if (exists)
        {
            throw new EntityAlreadyExistsException(entityName, propertyName, propertyValue);
        }
    }

    public static void AgainstInUse(bool inUse, string entityName, string? dependentEntity = null)
    {
        if (inUse)
        {
            throw new EntityInUseException(entityName, dependentEntity);
        }
    }

    #endregion
}
