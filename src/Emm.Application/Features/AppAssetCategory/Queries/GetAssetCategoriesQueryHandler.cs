using Emm.Application.Features.AppAssetCategory.Dtos;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Gridify;
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
        var queryRequest = request.QueryRequest;
        var query = _queryContext.Query<AssetCategory>()
            .ApplyFiltering(queryRequest)
            .OrderBy(q => q.Id)
            .AsQueryable();

        if (!string.IsNullOrEmpty(queryRequest.SearchTerm))
        {
            var searchTerm = queryRequest.SearchTerm.Trim();
            query = query
                .Where(q => EF.Functions.Like(q.Name, $"%{searchTerm}%")
                            || EF.Functions.Like(q.Description, $"%{searchTerm}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .ApplyOrderingAndPaging(queryRequest)
            .Select(x => new AssetCategoryResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                CreatedBy = _queryContext.Query<User>()
                    .Where(u => u.Id == x.CreatedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),
                UpdatedBy = _queryContext.Query<User>()
                    .Where(u => u.Id == x.UpdatedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),
                CreatedByUserId = x.CreatedByUserId,
                UpdatedByUserId = x.UpdatedByUserId
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult>.Success(request.QueryRequest.AsPagedResult(totalCount, items));
    }
}
