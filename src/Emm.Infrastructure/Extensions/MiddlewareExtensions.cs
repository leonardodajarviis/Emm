using Emm.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Emm.Infrastructure.Extensions;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Thêm middleware kiểm tra single device login
    /// Middleware này sẽ kiểm tra xem token có phải là session mới nhất hay không
    /// </summary>
    public static IApplicationBuilder UseSingleDeviceLogin(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SingleDeviceLoginMiddleware>();
    }
}
