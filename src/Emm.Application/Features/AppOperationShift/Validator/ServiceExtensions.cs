using Emm.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Emm.Application.Features.AppOperationShift.Validator;

public static class ServiceExtensions
{
    public static IServiceCollection AddAppOperationShiftValidators(this IServiceCollection services)
    {
        services.AddScoped<ICommandValidator<Commands.CreateOperationShiftCommand>, AssetsMustBeIdleValidator>();
        return services;
    }
}
