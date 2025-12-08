using Emm.Application.Features.AppAssetCategory.Dtos;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAssetCategory.Queries;

public class GetAssetCategoryByIdQueryHandler : IRequestHandler<GetAssetCategoryByIdQuery, Result<AssetCategoryResponse>>
{
    private readonly IQueryContext _queryContext;
    public GetAssetCategoryByIdQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<AssetCategoryResponse>> Handle(GetAssetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var assetCategory = await _queryContext.Query<AssetCategory>()
            .AsQueryable()
            .Where(x => x.Id == request.Id)
            .Select(x => new AssetCategoryResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                IsCodeGenerated = x.IsCodeGenerated,
                Description = x.Description,
                IsActive = x.IsActive,
                CreatedBy = _queryContext.Query<User>()
                    .Where(u => u.Id == x.CreatedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),
                UpdatedBy = _queryContext.Query<User>()
                    .Where(u => u.Id == x.UpdatedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),
                CreatedByUserId = x.CreatedByUserId,
                UpdatedByUserId = x.UpdatedByUserId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (assetCategory == null)
        {
            return Result<AssetCategoryResponse>.Failure(ErrorType.NotFound, "AssetCategory not found");
        }

        return Result<AssetCategoryResponse>.Success(assetCategory);
    }
}
