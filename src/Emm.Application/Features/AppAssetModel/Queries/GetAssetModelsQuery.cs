namespace Emm.Application.Features.AppAssetModel.Queries;

public record GetAssetModelsQuery(QueryParam QueryRequest) : IRequest<Result<PagedResult>>;
