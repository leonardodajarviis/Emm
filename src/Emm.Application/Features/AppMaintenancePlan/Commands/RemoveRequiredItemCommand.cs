namespace Emm.Application.Features.AppMaintenancePlan.Commands;

public record RemoveRequiredItemCommand(Guid MaintenancePlanDefinitionId, Guid RequiredItemId) : IRequest<Result>;
