using Emm.Application.Features.AppAssetCategory.Dtos;
using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAssetCategory.Queries;

public class GetAssetCategoriesQueryHandler : IRequestHandler<GetAssetCategoriesQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;

    public GetAssetCategoriesQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<PagedResult>> Handle(GetAssetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _queryContext.Query<AssetCategory>()
            .AsQueryable()
            .OrderBy(x => x.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.QueryRequest.Page - 1) * request.QueryRequest.PageSize)
            .Take(request.QueryRequest.PageSize)
            .Select(x => new AssetCategoryResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult>.Success(request.QueryRequest.AsPagedResult(totalCount, items));
    }
}
