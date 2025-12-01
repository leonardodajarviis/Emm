using Emm.Application.Features.AppParameterCatalog.Dtos;
using Emm.Application.Common;
using Emm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
        try
        {
            var query = _queryContext.Query<ParameterCatalog>()
                .OrderBy(pc => pc.Id);

            var totalCount = await query.CountAsync(cancellationToken);

            var parameterCatalogs = await query
                .Skip((request.QueryRequest.Page - 1) * request.QueryRequest.PageSize)
                .Take(request.QueryRequest.PageSize)
                .Select(pc => new ParameterCatalogResponse(
                    pc.Id,
                    pc.Code,
                    pc.Name,
                    pc.Description,
                    pc.CreatedAt,
                    pc.UpdatedAt
                ))
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult(
                request.QueryRequest.Page,
                request.QueryRequest.PageSize,
                totalCount,
                parameterCatalogs.Cast<object>().ToList()
            );

            return Result<PagedResult>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult>.Failure(ErrorType.Internal, ex.Message);
        }
    }
}
