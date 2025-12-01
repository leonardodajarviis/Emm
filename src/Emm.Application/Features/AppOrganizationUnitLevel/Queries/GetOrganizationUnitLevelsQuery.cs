using Emm.Application.Common;

namespace Emm.Application.Features.AppOrganizationUnitLevel.Queries;

public record GetOrganizationUnitLevelsQuery(QueryParam QueryRequest) : IRequest<Result<PagedResult>>;
