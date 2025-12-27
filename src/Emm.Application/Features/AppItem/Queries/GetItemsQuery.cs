namespace Emm.Application.Features.AppItem.Queries;

public record GetItemsQuery(QueryParam QueryRequest) : IRequest<Result<PagedResult>>;
