namespace Emm.Domain.Abstractions;

/// <summary>
/// Provides abstraction for DateTime operations to improve testability
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC date and time
    /// </summary>
    DateTime UtcNow { get; }
}
