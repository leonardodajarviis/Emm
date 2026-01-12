using Emm.Application.Abstractions;
using Emm.Application.Features.AppMaintenancePlan.Dtos;
using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppMaintenancePlan.Queries;

public class GetMaintenancePlanDefinitionsQueryHandler : IRequestHandler<GetMaintenancePlanDefinitionsQuery, Result<IEnumerable<MaintenancePlanDefinitionListItemResponse>>>
{
    private readonly IQueryContext _queryContext;

    public GetMaintenancePlanDefinitionsQueryHandler(IQueryContext queryContext)
    {
        ArgumentNullException.ThrowIfNull(queryContext);
        _queryContext = queryContext;
    }

    public async Task<Result<IEnumerable<MaintenancePlanDefinitionListItemResponse>>> Handle(GetMaintenancePlanDefinitionsQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = _queryContext.Query<MaintenancePlanDefinition>().AsQueryable();

        // Apply filters
        if (request.AssetModelId.HasValue)
        {
            query = query.Where(mp => mp.AssetModelId == request.AssetModelId.Value);
        }

        if (request.PlanType.HasValue)
        {
            query = query.Where(mp => mp.PlanType == request.PlanType.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(mp => mp.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(mp =>
                mp.Name.ToLower().Contains(searchTerm) ||
                (mp.Description != null && mp.Description.ToLower().Contains(searchTerm)));
        }

        var maintenancePlans = await query
            .Select(mp => new MaintenancePlanDefinitionListItemResponse
            {
                Id = mp.Id,
                Name = mp.Name,
                Description = mp.Description,
                PlanType = mp.PlanType,
                IsActive = mp.IsActive,
                AssetModelId = mp.AssetModelId,
                AssetModelName = _queryContext.Query<AssetModel>()
                    .Where(am => am.Id == mp.AssetModelId)
                    .Select(am => am.Name)
                    .FirstOrDefault(),
                RRule = mp.RRule,
                CreatedAt = mp.Audit.CreatedAt,
                ModifiedAt = mp.Audit.ModifiedAt,
                JobStepsCount = _queryContext.Query<MaintenancePlanJobStepDefinition>()
                    .Count(js => js.MaintenancePlanDefinitionId == mp.Id),
                RequiredItemsCount = _queryContext.Query<MaintenancePlanRequiredItem>()
                    .Count(ri => ri.MaintenancePlanDefinitionId == mp.Id)
            })
            .OrderByDescending(mp => mp.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<MaintenancePlanDefinitionListItemResponse>>.Success(maintenancePlans);
    }
}
