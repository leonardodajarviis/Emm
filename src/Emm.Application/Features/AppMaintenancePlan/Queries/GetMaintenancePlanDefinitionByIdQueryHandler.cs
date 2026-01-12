using Emm.Application.Abstractions;
using Emm.Application.Features.AppMaintenancePlan.Dtos;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Inventory;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppMaintenancePlan.Queries;

public class GetMaintenancePlanDefinitionByIdQueryHandler : IRequestHandler<GetMaintenancePlanDefinitionByIdQuery, Result<MaintenancePlanDefinitionResponse>>
{
    private readonly IQueryContext _queryContext;

    public GetMaintenancePlanDefinitionByIdQueryHandler(IQueryContext queryContext)
    {
        ArgumentNullException.ThrowIfNull(queryContext);
        _queryContext = queryContext;
    }

    public async Task<Result<MaintenancePlanDefinitionResponse>> Handle(GetMaintenancePlanDefinitionByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var maintenancePlan = await _queryContext.Query<MaintenancePlanDefinition>()
            .Where(mp => mp.Id == request.Id)
            .Select(mp => new MaintenancePlanDefinitionResponse
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
                CreatedBy = _queryContext.Query<User>()
                    .Where(u => u.Id == mp.Audit.CreatedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault() ?? "System",
                ModifiedBy = _queryContext.Query<User>()
                    .Where(u => u.Id == mp.Audit.ModifiedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),

                ParameterBasedTrigger = mp.ParameterBasedTrigger != null ? new ParameterBasedMaintenanceTriggerResponse
                {
                    Id = mp.ParameterBasedTrigger.Id,
                    ParameterId = mp.ParameterBasedTrigger.ParameterId,
                    ParameterName = _queryContext.Query<ParameterCatalog>()
                        .Where(p => p.Id == mp.ParameterBasedTrigger.ParameterId)
                        .Select(p => p.Name)
                        .FirstOrDefault(),
                    Value = mp.ParameterBasedTrigger.Value,
                    PlusTolerance = mp.ParameterBasedTrigger.PlusTolerance,
                    MinusTolerance = mp.ParameterBasedTrigger.MinusTolerance,
                    IsActive = mp.ParameterBasedTrigger.IsActive
                } : null,

                JobSteps = _queryContext.Query<MaintenancePlanJobStepDefinition>()
                    .Where(js => js.MaintenancePlanDefinitionId == mp.Id)
                    .Select(js => new MaintenancePlanJobStepDefinitionResponse
                    {
                        Id = js.Id,
                        MaintenancePlanDefinitionId = js.MaintenancePlanDefinitionId,
                        Name = js.Name,
                        OrganizationUnitId = js.OrganizationUnitId,
                        OrganizationUnitName = js.OrganizationUnitId != null ? _queryContext.Query<OrganizationUnit>()
                            .Where(ou => ou.Id == js.OrganizationUnitId)
                            .Select(ou => ou.Name)
                            .FirstOrDefault() : null,
                        Note = js.Note,
                        Order = js.Order
                    })
                    .OrderBy(js => js.Order)
                    .ToList(),

                RequiredItems = _queryContext.Query<MaintenancePlanRequiredItem>()
                    .Where(ri => ri.MaintenancePlanDefinitionId == mp.Id)
                    .Select(ri => new MaintenancePlanRequiredItemResponse
                    {
                        Id = ri.Id,
                        MaintenancePlanDefinitionId = ri.MaintenancePlanDefinitionId,
                        ItemGroupId = ri.ItemGroupId,
                        ItemGroupName = _queryContext.Query<ItemGroup>()
                            .Where(ig => ig.Id == ri.ItemGroupId)
                            .Select(ig => ig.Name)
                            .FirstOrDefault(),
                        ItemId = ri.ItemId,
                        ItemName = _queryContext.Query<Item>()
                            .Where(i => i.Id == ri.ItemId)
                            .Select(i => i.Name)
                            .FirstOrDefault(),
                        UnitOfMeasureId = ri.UnitOfMeasureId,
                        UnitOfMeasureName = _queryContext.Query<UnitOfMeasure>()
                            .Where(uom => uom.Id == ri.UnitOfMeasureId)
                            .Select(uom => uom.Name)
                            .FirstOrDefault(),
                        Quantity = ri.Quantity,
                        IsRequired = ri.IsRequired,
                        Note = ri.Note
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (maintenancePlan == null)
        {
            return Result<MaintenancePlanDefinitionResponse>.Failure(ErrorType.NotFound,
                $"Maintenance plan with ID {request.Id} not found.");
        }

        return Result<MaintenancePlanDefinitionResponse>.Success(maintenancePlan);
    }
}
