namespace Emm.Application.Features.AppMaintenancePlan.Commands;

public record AddRequiredItemCommand : IRequest<Result>
{
    public required Guid MaintenancePlanDefinitionId { get; init; }
    public required Guid ItemGroupId { get; init; }
    public required Guid ItemId { get; init; }
    public required Guid UnitOfMeasureId { get; init; }
    public required decimal Quantity { get; init; }
    public required bool IsRequired { get; init; }
    public string? Note { get; init; }
}
