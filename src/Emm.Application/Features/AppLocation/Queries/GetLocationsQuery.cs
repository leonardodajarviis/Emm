using Emm.Application.Common;

namespace Emm.Application.Features.AppLocation.Queries;

public record GetLocationsQuery(QueryParam QueryRequest) : IRequest<Result<PagedResult>>;
