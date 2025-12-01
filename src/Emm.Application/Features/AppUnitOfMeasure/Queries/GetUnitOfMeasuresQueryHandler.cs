using Emm.Application.Common;
using Emm.Application.Features.AppUnitOfMeasure.Dtos;
using Emm.Domain.Entities.Inventory;
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
        try
        {
            var query = _queryContext.Query<UnitOfMeasure>()
                .OrderBy(u => u.UnitType)
                .ThenBy(u => u.Name);

            var totalCount = await query.CountAsync(cancellationToken);

            var units = await query
                .Skip((request.QueryRequest.Page - 1) * request.QueryRequest.PageSize)
                .Take(request.QueryRequest.PageSize)
                .Select(u => new UnitOfMeasureListResponse(
                    u.Id,
                    u.Code,
                    u.Name,
                    u.Symbol,
                    u.UnitType,
                    u.UnitType.ToString(),
                    u.IsActive
                ))
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult(
                request.QueryRequest.Page,
                request.QueryRequest.PageSize,
                totalCount,
                units.Cast<object>().ToList()
            );

            return Result<PagedResult>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult>.Failure(ErrorType.Internal, ex.Message);
        }
    }
}
