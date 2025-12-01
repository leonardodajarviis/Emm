using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Queries;

public record GetUsersQuery(
    QueryParam QueryRequest
) : IRequest<Result<PagedResult>>;
