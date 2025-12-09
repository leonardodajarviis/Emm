using Emm.Application.Features.AppAssetType.Dtos;
using Emm.Application.Features.AppParameterCatalog.Dtos;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAssetType.Queries;

public class GetAssetTypeByIdQueryHandler : IRequestHandler<GetAssetTypeByIdQuery, Result<AssetTypeResponse>>
{
    private readonly IQueryContext _queryContext;

    public GetAssetTypeByIdQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<AssetTypeResponse>> Handle(GetAssetTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var assetType = await _queryContext.Query<AssetType>()
            .Where(x => x.Id == request.Id)
            .Select(at => new AssetTypeResponse
            {
                Id = at.Id,
                Code = at.Code,
                Name = at.Name,
                Description = at.Description,
                IsCodeGenerated = at.IsCodeGenerated,
                IsActive = at.IsActive,
                Parameters = _queryContext.Query<AssetTypeParameter>()
                    .Where(atp => atp.AssetTypeId == at.Id)
                    .Join(_queryContext.Query<ParameterCatalog>(), atp => atp.ParameterId, p => p.Id, (atp, p) => new AssetParameterResponse
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                    })
                    .ToList(),
                AssetCategoryId = at.AssetCategoryId,
                AssetCategoryName = _queryContext.Query<AssetCategory>()
                    .Where(ac => ac.Id == at.AssetCategoryId)
                    .Select(ac => ac.Name)
                    .FirstOrDefault(),
                CreatedBy = _queryContext.Query<User>()
                    .Where(u => u.Id == at.CreatedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),
                UpdatedBy = _queryContext.Query<User>()
                    .Where(u => u.Id == at.UpdatedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),
                UpdatedByUserId = at.UpdatedByUserId,
                CreatedByUserId = at.CreatedByUserId,
                CreatedAt = at.CreatedAt,
                UpdatedAt = at.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (assetType == null)
        {
            return Result<AssetTypeResponse>.Failure(ErrorType.NotFound, "AssetType not found.");
        }


        return Result<AssetTypeResponse>.Success(assetType);
    }
}
