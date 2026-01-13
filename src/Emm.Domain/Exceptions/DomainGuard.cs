namespace Emm.Domain.Exceptions;

/// <summary>
/// Guard clauses helper for Domain validation.
/// Giúp validate và throw appropriate domain exceptions.
/// </summary>
public static class DomainGuard
{
    public static void AgainstBusinessRule(bool condition, string rule, string message)
    {
        if (condition)
        {
            throw new DomainException(message, rule);
        }
    }

    public static void AgainstBusinessRule(Action<bool> conditionChecker, string rule, string message)
    {
        bool condition = false;
        conditionChecker.Invoke(condition);
        if (condition)
        {
            throw new DomainException(message, rule);
        }
    }

    public static T AgainstNotFound<T>(Func<T?> find, string message, string? rule = null)
        where T : class
    {
        var entity = find.Invoke();
        if (entity == null)
        {
            rule ??= "NotFound";
            throw new DomainException(message, rule);
        }
        return entity;
    }

    public static void AgainstNotFound(bool condition, string message, string? rule = null)
    {
        if (condition)
        {
            rule ??= "NotFound";
            throw new DomainException(message, rule);
        }
    }

    public static T AgainstMembershipRequired<T>(Func<T?> find, string message, string? rule = null)
        where T : class
    {
        var entity = find.Invoke();
        if (entity == null)
        {
            rule ??= "MembershipRequired";
            throw new DomainException(message, rule);
        }
        return entity;
    }
}
