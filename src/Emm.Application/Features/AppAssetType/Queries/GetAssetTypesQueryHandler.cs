using Emm.Application.Features.AppAssetType.Dtos;
using Emm.Application.Features.AppParameterCatalog.Dtos;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Gridify;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAssetType.Queries;

public class GetAssetTypesQueryHandler : IRequestHandler<GetAssetTypesQuery, Result<PagedResult>>
{
    private readonly IQueryContext _queryContext;

    public GetAssetTypesQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<PagedResult>> Handle(GetAssetTypesQuery request, CancellationToken cancellationToken)
    {
        var query = _queryContext.Query<AssetType>().ApplyFiltering(request.QueryRequest);

        var total = await query.CountAsync(cancellationToken);

        var result = await query
            .OrderBy(x => x.Id)
            .ApplyFilteringAndOrdering(request.QueryRequest)
            .Select(at => new AssetTypeResponse
            {
                Id = at.Id,
                Code = at.Code,
                Name = at.Name,
                Description = at.Description,
                IsActive = at.IsActive,
                AssetCategoryId = at.AssetCategoryId,
                Parameters = _queryContext.Query<AssetTypeParameter>()
                    .Where(atp => atp.AssetTypeId == at.Id)
                    .Join(_queryContext.Query<ParameterCatalog>(), atp => atp.ParameterId, p => p.Id, (atp, p) => new AssetParameterResponse
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                    })
                    .ToList(),
                AssetCategoryName = _queryContext.Query<AssetCategory>()
                    .Where(ac => ac.Id == at.AssetCategoryId)
                    .Select(ac => ac.Name)
                    .FirstOrDefault(),
                CreatedBy = _queryContext.Query<User>()
                    .Where(u => u.Id == at.CreatedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),
                UpdatedBy = _queryContext.Query<User>()
                    .Where(u => u.Id == at.UpdatedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),
                UpdatedByUserId = at.UpdatedByUserId,
                CreatedByUserId = at.CreatedByUserId,
                CreatedAt = at.CreatedAt,
                UpdatedAt = at.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult>.Success(request.QueryRequest.AsPagedResult(total, result));
    }
}
