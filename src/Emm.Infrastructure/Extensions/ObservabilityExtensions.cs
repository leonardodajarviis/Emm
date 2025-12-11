using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;

namespace Emm.Infrastructure.Extensions;


public static class ObservabilityExtensions
{
    // Currently no extensions defined here.
    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                // 1. Tự động thêm các Resource cơ bản (tên service, phiên bản)
                tracerProviderBuilder.AddSource(builder.Environment.ApplicationName);

                // 2. Thêm Instrumentation để tự động ghi lại các request HTTP đến
                // Mọi request đến Controller/API đều được ghi lại thành một Span
                tracerProviderBuilder.AddAspNetCoreInstrumentation();

                // 3. (Tùy chọn) Thêm Instrumentation để ghi lại các request HTTP đi (nếu dùng HttpClient)
                // tracerProviderBuilder.AddHttpClientInstrumentation();

                // 4. Cấu hình Exporter: Xuất dữ liệu Span ra Console
                tracerProviderBuilder.AddConsoleExporter();
            });

        return builder;
    }
}
