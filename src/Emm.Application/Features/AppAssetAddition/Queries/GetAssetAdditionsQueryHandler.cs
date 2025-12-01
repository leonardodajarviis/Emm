using Emm.Application.Features.AppAssetAddition.Dtos;
using Emm.Domain.Entities.AssetTransaction;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAssetAddition.Queries;

public class GetAssetAdditionsQueryHandler : IRequestHandler<GetAssetAdditionsQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;

    public GetAssetAdditionsQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<PagedResult>> Handle(GetAssetAdditionsQuery request, CancellationToken cancellationToken)
    {
        var query = _queryContext.Query<AssetAddition>()
            .AsQueryable()
            .OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.QueryRequest.Page - 1) * request.QueryRequest.PageSize)
            .Take(request.QueryRequest.PageSize)
            .Select(x => new AssetAdditionResponse
            {
                Id = x.Id,
                Code = x.Code,
                OrganizationUnitId = x.OrganizationUnitId,
                LocationId = x.LocationId,
                DecisionNumber = x.DecisionNumber,
                DecisionDate = x.DecisionDate,
                Reason = x.Reason,
                CreatedAt = x.CreatedAt,
                AssetAdditionLines = x.AssetAdditionLines.Select(line => new AssetAdditionLineResponse
                {
                    Id = line.Id,
                    AssetAdditionId = line.AssetAdditionId,
                    AssetModelId = line.AssetModelId,
                    AssetCode = line.AssetCode,
                    UnitPrice = line.UnitPrice
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult>.Success(request.QueryRequest.AsPagedResult(totalCount, items));
    }
}
