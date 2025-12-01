namespace Emm.Application.Features.AppAssetCategory.Queries;

public record GetAssetCategoryByIdQuery(long Id) : IRequest<Result<object>>;
