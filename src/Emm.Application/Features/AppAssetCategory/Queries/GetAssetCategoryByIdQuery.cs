using Emm.Application.Features.AppAssetCategory.Dtos;

namespace Emm.Application.Features.AppAssetCategory.Queries;

public record GetAssetCategoryByIdQuery(long Id) : IRequest<Result<AssetCategoryResponse>>;
