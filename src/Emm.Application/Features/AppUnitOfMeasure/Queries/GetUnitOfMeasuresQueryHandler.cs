using Emm.Application.Common;
using Emm.Application.Features.AppUnitOfMeasure.Dtos;
using Emm.Domain.Entities;
using Gridify;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppUnitOfMeasure.Queries;

public class GetUnitOfMeasuresQueryHandler : IRequestHandler<GetUnitOfMeasuresQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;

    public GetUnitOfMeasuresQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<PagedResult>> Handle(GetUnitOfMeasuresQuery request, CancellationToken cancellationToken)
    {
        var queryRequest = request.QueryRequest;

        var query = _queryContext.Query<UnitOfMeasure>()
            .ApplyFiltering(queryRequest)
            .OrderBy(u => u.UnitType)
            .ThenBy(u => u.Name).AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var units = await query
            .ApplyOrderingAndPaging(queryRequest)
            .Select(u => new UnitOfMeasureSummaryResponse(
                u.Id,
                u.Code,
                u.Name,
                u.Symbol,
                u.Description,
                u.UnitType,
                u.UnitType.ToString(),
                u.IsActive
            ))
            .ToListAsync(cancellationToken);

        return Result<PagedResult>.Success(queryRequest.AsPagedResult(totalCount, units));
    }
}
