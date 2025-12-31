using Emm.Application.Features.AppAssetAddition.Dtos;
using Emm.Domain.Entities.AssetTransaction;
using Emm.Domain.Entities.Organization;
using Gridify;
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
        var queryRequest = request.QueryRequest;

        var query = _queryContext.Query<AssetAddition>()
            .ApplyFiltering(queryRequest)
            .AsQueryable()
            .OrderByDescending(x => x.Audit.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .ApplyOrderingAndPaging(queryRequest)
            .Select(x => new AssetAdditionSummaryResponse
            {
                Id = x.Id,
                Code = x.Code.Value,
                OrganizationUnitId = x.OrganizationUnitId,
                OrganizationUnitName = _queryContext.Query<OrganizationUnit>()
                    .Where(ou => ou.Id == x.OrganizationUnitId)
                    .Select(ou => ou.Name)
                    .FirstOrDefault() ?? string.Empty,
                LocationId = x.LocationId,
                LocationName = _queryContext.Query<Location>()
                    .Where(ou => ou.Id == x.LocationId)
                    .Select(ou => ou.Name)
                    .FirstOrDefault() ?? string.Empty,
                DecisionNumber = x.DecisionNumber,
                DecisionDate = x.DecisionDate,
                Reason = x.Reason,
                CreatedAt = x.Audit.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult>.Success(request.QueryRequest.AsPagedResult(totalCount, items));
    }
}
