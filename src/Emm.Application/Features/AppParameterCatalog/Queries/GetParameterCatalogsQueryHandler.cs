using Emm.Application.Features.AppParameterCatalog.Dtos;
using Emm.Application.Common;
using Emm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Gridify;

namespace Emm.Application.Features.AppParameterCatalog.Queries;

public class GetParameterCatalogsQueryHandler : IRequestHandler<GetParameterCatalogsQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;

    public GetParameterCatalogsQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<PagedResult>> Handle(GetParameterCatalogsQuery request, CancellationToken cancellationToken)
    {
        var searchTerm = request.QueryRequest.SearchTerm?.Trim();
        var queryRequest = request.QueryRequest;

        var query = _queryContext.Query<ParameterCatalog>()
            .ApplyFiltering(queryRequest)
            .OrderBy(pc => pc.Id).AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(pc => EF.Functions.Like(pc.Name, $"%{searchTerm}%")
                                     || EF.Functions.Like(pc.Code, $"%{searchTerm}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var parameterCatalogs = await query
            .ApplyOrderingAndPaging(request.QueryRequest)
            .Select(pc => new ParameterCatalogResponse(
                pc.Id,
                pc.Code,
                pc.Name,
                _queryContext.Query<UnitOfMeasure>()
                    .Where(uom => uom.Id == pc.UnitOfMeasureId)
                    .Select(uom => uom.Name)
                    .FirstOrDefault(),
                pc.Description,
                pc.Audit.CreatedAt,
                pc.Audit.ModifiedAt
            ))
            .ToListAsync(cancellationToken);

        return Result<PagedResult>.Success(queryRequest.AsPagedResult(totalCount, parameterCatalogs));
    }
}
