using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Emm.Presentation.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Fixed window rate limiter for login endpoint
            options.AddFixedWindowLimiter("login", limiterOptions =>
            {
                limiterOptions.PermitLimit = 5; // 5 requests
                limiterOptions.Window = TimeSpan.FromMinutes(1); // per minute
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 2; // Allow 2 requests to queue
            });

            // General API rate limiter
            options.AddFixedWindowLimiter("api", limiterOptions =>
            {
                limiterOptions.PermitLimit = 100; // 100 requests
                limiterOptions.Window = TimeSpan.FromMinutes(1); // per minute
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 10;
            });

            // Configure rejection behavior
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    await context.HttpContext.Response.WriteAsync(
                        $"Too many requests. Please try again after {retryAfter.TotalSeconds} seconds.",
                        cancellationToken: token);
                }
                else
                {
                    await context.HttpContext.Response.WriteAsync(
                        "Too many requests. Please try again later.",
                        cancellationToken: token);
                }
            };
        });

        return services;
    }
}
