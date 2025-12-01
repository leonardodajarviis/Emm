using Emm.Application.Features.AppAsset.Dtos;
using Emm.Domain.Entities.AssetCatalog;
using Gridify;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAsset.Queries;

public class GetAssetsQueryHandler : IRequestHandler<GetAssetsQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;

    public GetAssetsQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<PagedResult>> Handle(GetAssetsQuery request, CancellationToken cancellationToken)
    {
        var query = _queryContext.Query<Asset>()
            .AsQueryable()
            .OrderBy(x => x.Code);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .ApplyPaging(request.QueryRequest)
            .Take(request.QueryRequest.PageSize)
            .Select(x => new AssetSummaryResponse
            {
                Id = x.Id,
                Code = x.Code,
                DisplayName = x.DisplayName,
                AssetModelId = x.AssetModelId,
                Description = x.Description,
                Status = (int)x.Status,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult>.Success(request.QueryRequest.AsPagedResult(totalCount, items));
    }
}
