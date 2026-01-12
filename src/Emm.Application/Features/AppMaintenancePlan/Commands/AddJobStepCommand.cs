namespace Emm.Application.Features.AppMaintenancePlan.Commands;

public record AddJobStepCommand : IRequest<Result>
{
    public required Guid MaintenancePlanDefinitionId { get; init; }
    public required string Name { get; init; }
    public Guid? OrganizationUnitId { get; init; }
    public string? Note { get; init; }
    public int Order { get; init; }
}
