using System.Runtime.InteropServices;
using Emm.Domain.Abstractions;

namespace Emm.Infrastructure.Services;

/// <summary>
/// Default implementation of IDateTimeProvider that uses system DateTime
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    private const string WindowsTzId = "SE Asia Standard Time";
    private const string LinuxTzId = "Asia/Ho_Chi_Minh";

    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Now => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, GetTimeZone()).DateTime;

    private static TimeZoneInfo GetTimeZone()
    {
        var tzId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? WindowsTzId
            : LinuxTzId;

        return TimeZoneInfo.FindSystemTimeZoneById(tzId);
    }
}
