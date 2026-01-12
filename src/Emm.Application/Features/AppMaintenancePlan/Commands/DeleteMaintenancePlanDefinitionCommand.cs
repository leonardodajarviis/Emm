namespace Emm.Application.Features.AppMaintenancePlan.Commands;

public record DeleteMaintenancePlanDefinitionCommand(Guid Id) : IRequest<Result>;
