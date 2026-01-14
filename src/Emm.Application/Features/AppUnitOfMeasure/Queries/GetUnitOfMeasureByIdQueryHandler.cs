using Emm.Application.Common;
using Emm.Application.Features.AppUnitOfMeasure.Dtos;
using Emm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppUnitOfMeasure.Queries;

public class GetUnitOfMeasureByIdQueryHandler : IRequestHandler<GetUnitOfMeasureByIdQuery, Result<object>>
{
    private readonly IQueryContext _queryContext;

    public GetUnitOfMeasureByIdQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<object>> Handle(GetUnitOfMeasureByIdQuery request, CancellationToken cancellationToken)
    {
        var unitOfMeasure = await _queryContext.Query<UnitOfMeasure>()
            .Where(u => u.Id == request.Id)
            .Select(u => new UnitOfMeasureSummaryResponse(
                u.Id,
                u.Code,
                u.Name,
                u.Symbol,
                u.Description,
                u.IsActive
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (unitOfMeasure == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Unit of measure not found");
        }

        return Result<object>.Success(unitOfMeasure);
    }
}
