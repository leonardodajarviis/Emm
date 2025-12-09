using Emm.Application.Abstractions;
using Emm.Application.Features.AppAssetModel.Dtos;
using Emm.Domain.Entities.AssetCatalog;
using Gridify;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAssetModel.Queries;

public class GetAssetModelsQueryHandler : IRequestHandler<GetAssetModelsQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;
    private readonly IFileStorage _fileStorage;

    public GetAssetModelsQueryHandler(IQueryContext queryContext, IFileStorage fileStorage)
    {
        _queryContext = queryContext;
        _fileStorage = fileStorage;
    }

    public async Task<Result<PagedResult>> Handle(GetAssetModelsQuery request, CancellationToken cancellationToken)
    {
        var query = _queryContext.Query<AssetModel>().ApplyFiltering(request.QueryRequest);

        if (!string.IsNullOrEmpty(request.QueryRequest.GetSearch()))
        {
            var search = request.QueryRequest.GetSearch()!;
            query = query.Where(x => x.Code.Value.Contains(search) || x.Name.Contains(search));
        }

        var total = await query.CountAsync(cancellationToken);

        var result = await query
            .OrderBy(x => x.Id)
            .Select(x => new AssetModelSummaryResponse
            {
                Id = x.Id,
                Code = x.Code.Value,
                Name = x.Name,
                Description = x.Description,
                Notes = x.Notes,
                ParentId = x.ParentId,
                AssetCategoryId = x.AssetCategoryId,
                ThumbnailUrl = _fileStorage.GetFileUrl(x.ThumbnailUrl ?? ""),
                AssetTypeId = x.AssetTypeId,
                IsActive = x.IsActive,
                CreatedAt = x.Audit.CreatedAt,
                ModifiedAt = x.Audit.ModifiedAt,
                ParentName = _queryContext.Query<AssetModel>()
                    .Where(am => am.Id == x.ParentId)
                    .Select(am => am.Name)
                    .FirstOrDefault(),
                AssetCategoryName = _queryContext.Query<AssetCategory>()
                    .Where(ac => ac.Id == x.AssetCategoryId)
                    .Select(ac => ac.Name)
                    .FirstOrDefault(),
                AssetTypeName = _queryContext.Query<AssetType>()
                    .Where(at => at.Id == x.AssetTypeId)
                    .Select(at => at.Name)
                    .FirstOrDefault(),
            })
            .OrderByDescending(x => x.CreatedAt)
            .ApplyOrderingAndPaging(request.QueryRequest)
            .ToListAsync(cancellationToken);

        return Result<PagedResult>.Success(request.QueryRequest.AsPagedResult(total, result));
    }
}
