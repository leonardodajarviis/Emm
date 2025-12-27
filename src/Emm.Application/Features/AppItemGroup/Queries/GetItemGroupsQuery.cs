namespace Emm.Application.Features.AppItemGroup.Queries;

public record GetItemGroupsQuery(QueryParam QueryRequest) : IRequest<Result<PagedResult>>;
