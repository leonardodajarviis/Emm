namespace Emm.Application.Features.AppAsset.Queries;

public record GetAssetsQuery(QueryParam QueryRequest) : IRequest<Result<PagedResult>>;
