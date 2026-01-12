namespace Emm.Application.Features.AppMaintenancePlan.Commands;

public record UpdateRequiredItemCommand : IRequest<Result>
{
    public required Guid MaintenancePlanDefinitionId { get; init; }
    public required Guid RequiredItemId { get; init; }
    public required decimal Quantity { get; init; }
    public required bool IsRequired { get; init; }
    public string? Note { get; init; }
}
