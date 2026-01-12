namespace Emm.Application.Features.AppMaintenancePlan.Commands;

public record RemoveJobStepCommand(Guid MaintenancePlanDefinitionId, Guid JobStepId) : IRequest<Result>;
