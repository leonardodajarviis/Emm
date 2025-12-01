using Emm.Application.Common;
using Emm.Application.Features.AppUnitOfMeasure.Dtos;
using Emm.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppUnitOfMeasure.Queries;

public class GetUnitOfMeasuresByTypeQueryHandler : IRequestHandler<GetUnitOfMeasuresByTypeQuery, Result<object>>
{
    private readonly IQueryContext _queryContext;

    public GetUnitOfMeasuresByTypeQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<object>> Handle(GetUnitOfMeasuresByTypeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var units = await _queryContext.Query<UnitOfMeasure>()
                .Where(u => u.UnitType == request.UnitType && u.IsActive)
                .OrderBy(u => u.Name)
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

            return Result<object>.Success(units);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure(ErrorType.Internal, ex.Message);
        }
    }
}
