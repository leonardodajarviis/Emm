using Emm.Domain.Abstractions;

namespace Emm.Infrastructure.Services;

/// <summary>
/// Default implementation of IDateTimeProvider that uses system DateTime
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
