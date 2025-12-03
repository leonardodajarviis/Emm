using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Queries;

public record GetRolesQuery(
    QueryParam QueryRequest
) : IRequest<Result<PagedResult>>;
