using Emm.Application.ErrorCodes;
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
                Code = x.Code.ToString(),
                Name = x.Name,
                IsCodeGenerated = x.IsCodeGenerated,
                Description = x.Description,
                IsActive = x.IsActive,
                CreatedBy = _queryContext.Query<User>()
                    .Where(u => u.Id == x.Audit.CreatedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),
                ModifiedBy = _queryContext.Query<User>()
                    .Where(u => u.Id == x.Audit.ModifiedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),
                CreatedByUserId = x.Audit.CreatedByUserId,
                ModifiedByUserId = x.Audit.ModifiedByUserId,
                CreatedAt = x.Audit.CreatedAt,
                ModifiedAt = x.Audit.ModifiedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (assetCategory == null)
        {
            return Result<AssetCategoryResponse>.Failure(ErrorType.NotFound, "AssetCategory not found", AssetCategoryErrorCodes.NotFound);
        }

        return Result<AssetCategoryResponse>.Success(assetCategory);
    }
}
