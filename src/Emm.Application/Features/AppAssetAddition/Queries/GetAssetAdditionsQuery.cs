namespace Emm.Application.Features.AppAssetAddition.Queries;

public record GetAssetAdditionsQuery(QueryParam QueryRequest) : IRequest<Result<PagedResult>>;
