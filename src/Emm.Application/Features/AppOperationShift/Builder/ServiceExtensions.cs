using Microsoft.Extensions.DependencyInjection;

namespace Emm.Application.Features.AppOperationShift.Builder;


public static class ServiceExtensions
{
    public static IServiceCollection AddShiftLogBuilderHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICreateShiftLogBuilderHandler, AddShiftLogCheckpointHandler>();
        services.AddScoped<ICreateShiftLogBuilderHandler, AddShiftLogItemsHandler>();
        services.AddScoped<ICreateShiftLogBuilderHandler, AddShiftLogReadingsHandler>();
        services.AddScoped<ICreateShiftLogBuilderHandler, AddShiftLogEventHandler>();

        services.AddScoped<IUpdateShiftLogBuilderHandler, SyncShiftLogCheckpointHandler>();
        services.AddScoped<IUpdateShiftLogBuilderHandler, SyncShiftLogItemsHandler>();
        services.AddScoped<IUpdateShiftLogBuilderHandler, SyncShiftLogReadingsHandler>();
        services.AddScoped<IUpdateShiftLogBuilderHandler, SyncShiftLogEventsHandler>();

        return services;
    }
}
