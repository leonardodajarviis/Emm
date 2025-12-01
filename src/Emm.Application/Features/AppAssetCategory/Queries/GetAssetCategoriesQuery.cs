namespace Emm.Application.Features.AppAssetCategory.Queries;

public record GetAssetCategoriesQuery(QueryParam QueryRequest) : IRequest<Result<PagedResult>>;
