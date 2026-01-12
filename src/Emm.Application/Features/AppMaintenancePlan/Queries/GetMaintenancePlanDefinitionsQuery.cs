using Emm.Application.Features.AppMaintenancePlan.Dtos;
using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppMaintenancePlan.Queries;

public record GetMaintenancePlanDefinitionsQuery : IRequest<Result<IEnumerable<MaintenancePlanDefinitionListItemResponse>>>
{
    public Guid? AssetModelId { get; init; }
    public MaintenancePlanType? PlanType { get; init; }
    public bool? IsActive { get; init; }
    public string? SearchTerm { get; init; }
}
