namespace Emm.Application.Features.AppOperationShift.Queries;

public record GetOperationShiftsQuery(
    QueryParam QueryRequest
) : IRequest<Result<PagedResult>>;
