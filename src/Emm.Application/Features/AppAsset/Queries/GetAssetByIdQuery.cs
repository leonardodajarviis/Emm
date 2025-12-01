using Emm.Application.Features.AppAsset.Dtos;

namespace Emm.Application.Features.AppAsset.Queries;

public record GetAssetByIdQuery(long Id) : IRequest<Result<AssetResponse>>;
