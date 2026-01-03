using Emm.Application.Features.AppAssetAddition.Dtos;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.AssetTransaction;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAssetAddition.Queries;

public class GetAssetAdditionByIdQueryHandler : IRequestHandler<GetAssetAdditionByIdQuery, Result<object>>
{
    private readonly IQueryContext _queryContext;

    public GetAssetAdditionByIdQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<object>> Handle(GetAssetAdditionByIdQuery request, CancellationToken cancellationToken)
    {
        var assetAddition = await _queryContext.Query<AssetAddition>()
            .Where(x => x.Id == request.Id)
            .Select(x => new AssetAdditionResponse
            {
                Id = x.Id,
                Code = x.Code.Value,
                OrganizationUnitId = x.OrganizationUnitId,
                OrganizationUnitName = _queryContext.Query<OrganizationUnit>()
                    .Where(ou => ou.Id == x.OrganizationUnitId)
                    .Select(ou => ou.Name)
                    .FirstOrDefault(),
                LocationId = x.LocationId,
                LocationName = _queryContext.Query<Location>()
                    .Where(loc => loc.Id == x.LocationId)
                    .Select(loc => loc.Name)
                    .FirstOrDefault(),
                DecisionNumber = x.DecisionNumber,
                DecisionDate = x.DecisionDate,
                Reason = x.Reason,
                CreatedAt = x.Audit.CreatedAt,
                AssetAdditionLines = _queryContext.Query<AssetAdditionLine>()
                    .Where(line => line.AssetAdditionId == x.Id)
                    .Select(line => new AssetAdditionLineResponse
                    {
                        Id = line.Id,
                        AssetAdditionId = line.AssetAdditionId,
                        AssetModelId = line.AssetModelId,
                        AssetModelName = _queryContext.Query<AssetModel>()
                            .Where(am => am.Id == line.AssetModelId)
                            .Select(am => am.Name)
                            .FirstOrDefault(),
                        AssetCode = line.AssetCode.Value,
                        UnitPrice = line.UnitPrice
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (assetAddition == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "AssetAddition not found");
        }

        return Result<object>.Success(assetAddition);
    }
}
