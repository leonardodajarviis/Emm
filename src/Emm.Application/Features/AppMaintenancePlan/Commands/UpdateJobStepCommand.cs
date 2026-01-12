namespace Emm.Application.Features.AppMaintenancePlan.Commands;

public record UpdateJobStepCommand : IRequest<Result>
{
    public required Guid MaintenancePlanDefinitionId { get; init; }
    public required Guid JobStepId { get; init; }
    public required string Name { get; init; }
    public string? Note { get; init; }
    public int Order { get; init; }
}
