namespace Emm.Application.Features.AppAssetType.Queries;

public record GetAssetTypesQuery(QueryParam QueryRequest) : IRequest<Result<PagedResult>>;
