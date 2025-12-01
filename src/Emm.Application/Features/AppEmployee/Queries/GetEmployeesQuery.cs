using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppEmployee.Queries;

public record GetEmployeesQuery(
    QueryParam QueryRequest
) : IRequest<Result<PagedResult>>;
