using Emm.Application.Features.AppParameterCatalog.Dtos;
using Emm.Application.Common;
using Emm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppParameterCatalog.Queries;

public class GetParameterCatalogByIdQueryHandler : IRequestHandler<GetParameterCatalogByIdQuery, Result<ParameterCatalogResponse>>
{
    private readonly IQueryContext _queryContext;

    public GetParameterCatalogByIdQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<ParameterCatalogResponse>> Handle(GetParameterCatalogByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var parameterCatalog = await _queryContext.Query<ParameterCatalog>()
                .FirstOrDefaultAsync(pc => pc.Id == request.Id, cancellationToken);

            if (parameterCatalog is null)
            {
                return Result<ParameterCatalogResponse>.Failure(ErrorType.NotFound, "ParameterCatalog not found");
            }

            var response = new ParameterCatalogResponse(
                parameterCatalog.Id,
                parameterCatalog.Code,
                parameterCatalog.Name,
                _queryContext.Query<UnitOfMeasure>()
                    .Where(uom => uom.Id == parameterCatalog.UnitOfMeasureId)
                    .Select(uom => uom.Name)
                    .FirstOrDefault(),
                parameterCatalog.Description,
                parameterCatalog.Audit.CreatedAt,
                parameterCatalog.Audit.ModifiedAt
            );

            return Result<ParameterCatalogResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<ParameterCatalogResponse>.Failure(ErrorType.Internal, ex.Message);
        }
    }
}
