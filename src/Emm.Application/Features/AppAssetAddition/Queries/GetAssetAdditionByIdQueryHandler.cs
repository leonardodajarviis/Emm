using Emm.Application.Features.AppAssetAddition.Dtos;
using Emm.Domain.Entities.AssetTransaction;
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
            .FirstOrDefaultAsync(cancellationToken);

        if (assetAddition == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "AssetAddition not found");
        }

        return Result<object>.Success(assetAddition);
    }
}
