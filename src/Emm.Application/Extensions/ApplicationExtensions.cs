using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using LazyNet.Symphony.Extensions;
using Emm.Application.Behaviors;
using Emm.Application.Common;

namespace Emm.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediator(assembly)
            .AddPipelineBehavior(typeof(DomainExceptionHandlerBehavior<,>));
            // .AddPipelineBehavior(typeof(AuthorizationBehavior<,>)); // Authorization at Application layer

        // Register validation services
        services.AddScoped<IForeignKeyValidator, ForeignKeyValidator>();

        return services;
    }
}
