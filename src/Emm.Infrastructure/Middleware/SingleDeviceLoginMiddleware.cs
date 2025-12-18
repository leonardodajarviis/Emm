using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Emm.Infrastructure.Middleware;

/// <summary>
/// Middleware kiểm tra xem token có phải là session mới nhất của user hay không
/// Chỉ hoạt động khi IsMultiDeviceLoginAllowed = false
/// </summary>
public class SingleDeviceLoginMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtOptions _jwtOptions;

    public SingleDeviceLoginMiddleware(RequestDelegate next, IOptions<JwtOptions> jwtOptions)
    {
        _next = next;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context, IUserSessionRepository userSessionRepository, IUnitOfWork unitOfWork)
    {
        // Nếu cho phép multi-device login thì skip middleware này
        if (_jwtOptions.IsMultiDeviceLoginAllowed)
        {
            await _next(context);
            return;
        }

        // Chỉ kiểm tra khi user đã authenticated
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var jtiClaim = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(jtiClaim) && !string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
            {
                // Lấy session hiện tại dựa trên JTI
                // GetByAccessTokenJtiAsync already filters RevokedAt == null, so no need to check IsRevoked
                var currentSession = await userSessionRepository.GetByAccessTokenJtiAsync(jtiClaim);

                if (currentSession == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        error = new
                        {
                            code = "SESSION_INVALID",
                            message = "Your session is invalid or has been revoked."
                        }
                    }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
                    return;
                }

                // Lấy tất cả active sessions của user
                var activeSessions = await userSessionRepository.GetActiveSessionsByUserIdAsync(userId);
                var latestSession = activeSessions
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstOrDefault();

                // Nếu session hiện tại không phải là session mới nhất
                if (latestSession != null && latestSession.Id != currentSession.Id)
                {
                    // Revoke session hiện tại
                    currentSession.Revoke();
                    await unitOfWork.SaveChangesAsync();

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        error = new
                        {
                            code = "SESSION_EXPIRED",
                            message = "Your account has been logged in from another device. Please login again."
                        }
                    }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
                    return;
                }
            }
        }

        await _next(context);
    }
}
