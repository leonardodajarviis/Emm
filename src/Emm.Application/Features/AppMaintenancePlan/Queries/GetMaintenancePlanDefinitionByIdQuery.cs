using Emm.Application.Features.AppMaintenancePlan.Dtos;

namespace Emm.Application.Features.AppMaintenancePlan.Queries;

public record GetMaintenancePlanDefinitionByIdQuery(Guid Id) : IRequest<Result<MaintenancePlanDefinitionResponse>>;
