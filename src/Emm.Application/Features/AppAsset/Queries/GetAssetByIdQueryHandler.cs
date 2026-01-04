using System.Reflection.Metadata;
using Emm.Application.Features.AppAsset.Dtos;
using Emm.Application.Features.AppAssetModel.Dtos;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Inventory;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAsset.Queries;

public class GetAssetByIdQueryHandler : IRequestHandler<GetAssetByIdQuery, Result<AssetResponse>>
{
    private readonly IQueryContext _qq;

    public GetAssetByIdQueryHandler(IQueryContext queryContext)
    {
        _qq = queryContext;
    }

    public async Task<Result<AssetResponse>> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
    {
        var asset = await _qq.Query<Asset>()
            .AsQueryable()
            .Where(x => x.Id == request.Id)
            .Select(x => new AssetResponse
            {
                Id = x.Id,
                Code = x.Code.Value,
                IsCodeGenerated = x.IsCodeGenerated,
                DisplayName = x.DisplayName,
                AssetCategoryId = x.AssetCategoryId,
                AssetCategoryCode = x.AssetCategoryCode,
                AssetCategoryName = x.AssetCategoryName,
                LocationId = x.LocationId,
                LocationName = _qq.Query<Location>()
                    .Where(loc => loc.Id == x.LocationId)
                    .Select(loc => loc.Name)
                    .FirstOrDefault(),
                OrganizationUnitId = x.OrganizationUnitId,
                OrganizationUnitName = _qq.Query<OrganizationUnit>()
                    .Where(ou => ou.Id == x.OrganizationUnitId)
                    .Select(ou => ou.Name)
                    .FirstOrDefault(),
                AssetModelId = x.AssetModelId,
                AssetModelCode = x.AssetModelCode,
                AssetModelName = x.AssetModelName,
                AssetTypeId = x.AssetTypeId,
                AssetTypeCode = x.AssetTypeCode,
                AssetTypeName = x.AssetTypeName,
                Description = x.Description,
                Status = x.Status.Value,
                CreatedAt = x.Audit.CreatedAt,
                ModifiedAt = x.Audit.ModifiedAt,

                Parameters = _qq.Query<AssetParameter>()
                    .Where(ap => ap.AssetId == x.Id)
                    .Select(ap => new AssetParameterResponse
                    {
                        AssetId = ap.AssetId,
                        ParameterId = ap.ParameterId,
                        ParameterCode = ap.ParameterCode,
                        ParameterName = ap.ParameterName,
                        ParameterUnit = ap.ParameterUnit,
                        CurrentValue = ap.CurrentValue,
                        IsMaintenanceParameter = ap.IsMaintenanceParameter,
                        ValueToMaintenance = ap.ValueToMaintenance
                    })
                    .ToList(),

                // Lấy Maintenance Plan Definitions từ AssetModel
                MaintenancePlanDefinitions = _qq.Query<MaintenancePlanDefinition>()
                    .Where(mp => mp.AssetModelId == x.AssetModelId)
                    .Select(mp => new MaintenancePlanDefinitionResponse
                    {
                        Id = mp.Id,
                        Name = mp.Name,
                        Description = mp.Description,
                        PlanType = mp.PlanType,
                        IsActive = mp.IsActive,
                        AssetModelId = mp.AssetModelId,
                        RRule = mp.RRule,
                        CreatedAt = mp.Audit.CreatedAt,
                        ModifiedAt = mp.Audit.ModifiedAt,

                        ParameterBasedTrigger = mp.ParameterBasedTrigger != null ? new ParameterBasedMaintenanceTriggerResponse
                        {
                            Id = mp.ParameterBasedTrigger.Id,
                            ParameterId = mp.ParameterBasedTrigger.ParameterId,
                            TriggerValue = mp.ParameterBasedTrigger.TriggerValue,
                            MinValue = mp.ParameterBasedTrigger.MinValue,
                            MaxValue = mp.ParameterBasedTrigger.MaxValue,
                            TriggerCondition = mp.ParameterBasedTrigger.TriggerCondition,
                            IsActive = mp.ParameterBasedTrigger.IsActive
                        } : null,

                        JobSteps = _qq.Query<MaintenancePlanJobStepDefinition>()
                            .Where(js => js.MaintenancePlanDefinitionId == mp.Id)
                            .Select(js => new MaintenancePlanJobStepDefinitionResponse
                            {
                                Id = js.Id,
                                MaintenancePlanDefinitionId = js.MaintenancePlanDefinitionId,
                                Name = js.Name,
                                OrganizationUnitId = js.OrganizationUnitId,
                                OrganizationUnit = js.OrganizationUnitId != null ? _qq.Query<OrganizationUnit>()
                                    .Where(ou => ou.Id == js.OrganizationUnitId)
                                    .Select(ou => new OrganizationUnitResponse
                                    {
                                        Id = ou.Id,
                                        Code = ou.Code,
                                        Name = ou.Name
                                    })
                                    .FirstOrDefault() : null,
                                Note = js.Note,
                                Order = js.Order,
                            })
                            .ToList(),

                        RequiredItems = _qq.Query<MaintenancePlanRequiredItem>()
                            .Where(ri => ri.MaintenancePlanDefinitionId == mp.Id)
                            .Select(ri => new MaintenancePlanRequiredItemResponse
                            {
                                Id = ri.Id,
                                MaintenancePlanDefinitionId = ri.MaintenancePlanDefinitionId,
                                ItemGroupId = ri.ItemGroupId,
                                ItemGroupName = _qq.Query<ItemGroup>()
                                    .Where(ig => ig.Id == ri.ItemGroupId)
                                    .Select(ig => ig.Name)
                                    .FirstOrDefault(),
                                ItemId = ri.ItemId,
                                ItemName = _qq.Query<Item>()
                                    .Where(i => i.Id == ri.ItemId)
                                    .Select(i => i.Name)
                                    .FirstOrDefault(),
                                UnitOfMeasureId = ri.UnitOfMeasureId,
                                UnitOfMeasureName = _qq.Query<UnitOfMeasure>()
                                    .Where(u => u.Id == ri.UnitOfMeasureId)
                                    .Select(u => u.Name)
                                    .FirstOrDefault(),
                                Quantity = ri.Quantity,
                                IsRequired = ri.IsRequired,
                                Note = ri.Note
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (asset == null)
        {
            return Result<AssetResponse>.Failure(ErrorType.NotFound, "Asset not found");
        }

        return Result<AssetResponse>.Success(asset);
    }
}
