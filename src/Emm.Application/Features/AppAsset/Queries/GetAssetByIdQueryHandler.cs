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
        // Query 1: Asset chính với Location và OrganizationUnit
        var asset = await (
            from x in _qq.Query<Asset>().AsQueryable()
            join loc in _qq.Query<Location>() on x.LocationId equals loc.Id into locGroup
            from loc in locGroup.DefaultIfEmpty()
            join ou in _qq.Query<OrganizationUnit>() on x.OrganizationUnitId equals ou.Id into ouGroup
            from ou in ouGroup.DefaultIfEmpty()
            where x.Id == request.Id
            select new AssetResponse
            {
                Id = x.Id,
                Code = x.Code.Value,
                IsCodeGenerated = x.IsCodeGenerated,
                DisplayName = x.DisplayName,
                AssetCategoryId = x.AssetCategoryId,
                AssetCategoryCode = x.AssetCategoryCode,
                AssetCategoryName = x.AssetCategoryName,
                LocationId = x.LocationId,
                LocationName = loc.Name,
                OrganizationUnitId = x.OrganizationUnitId,
                OrganizationUnitName = ou.Name,
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
            }
        ).FirstOrDefaultAsync(cancellationToken);

        if (asset == null)
        {
            return Result<AssetResponse>.Failure(ErrorType.NotFound, "Asset not found");
        }

        // Query 2: Parameters với UnitOfMeasure
        asset.Parameters = await (
            from ap in _qq.Query<AssetParameter>()
            join u in _qq.Query<UnitOfMeasure>() on ap.UnitOfMeasureId equals u.Id into uGroup
            from u in uGroup.DefaultIfEmpty()
            where ap.AssetId == request.Id
            select new AssetParameterResponse
            {
                AssetId = ap.AssetId,
                ParameterId = ap.ParameterId,
                ParameterCode = ap.ParameterCode,
                ParameterName = ap.ParameterName,
                CurrentValue = ap.CurrentValue,
                IsMaintenanceParameter = ap.IsMaintenanceParameter,
                UnitOfMeasureId = ap.UnitOfMeasureId,
                UnitOfMeasureName = u.Name ?? "",
                UnitOfMeasureSymbol = u.Symbol ?? ""
            }
        ).ToListAsync(cancellationToken);

        // Query 3: MaintenancePlans (nếu có AssetModelId)
        var maintenancePlans = await (
            from mp in _qq.Query<MaintenancePlanDefinition>()
            where mp.AssetModelId == asset.AssetModelId
            select new MaintenancePlanDefinitionResponse
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
                    TriggerValue = mp.ParameterBasedTrigger.Value,
                    MinValue = mp.ParameterBasedTrigger.PlusTolerance,
                    MaxValue = mp.ParameterBasedTrigger.MinusTolerance,
                    TriggerCondition = mp.ParameterBasedTrigger.TriggerCondition,
                    IsActive = mp.ParameterBasedTrigger.IsActive
                } : null,
                JobSteps = new List<MaintenancePlanJobStepDefinitionResponse>(),
                RequiredItems = new List<MaintenancePlanRequiredItemResponse>()
            }
        ).ToListAsync(cancellationToken);

        if (maintenancePlans.Any())
        {
            var planIds = maintenancePlans.Select(mp => mp.Id).ToArray();

            // Query 4: Tất cả JobSteps cho các MaintenancePlans
            var allJobSteps = await (
                from js in _qq.Query<MaintenancePlanJobStepDefinition>()
                join ou2 in _qq.Query<OrganizationUnit>() on js.OrganizationUnitId equals ou2.Id into ou2Group
                from ou2 in ou2Group.DefaultIfEmpty()
                where planIds.Contains(js.MaintenancePlanDefinitionId)
                select new
                {
                    JobStep = new MaintenancePlanJobStepDefinitionResponse
                    {
                        Id = js.Id,
                        MaintenancePlanDefinitionId = js.MaintenancePlanDefinitionId,
                        Name = js.Name,
                        OrganizationUnitId = js.OrganizationUnitId,
                        OrganizationUnit = js.OrganizationUnitId != null ? new OrganizationUnitResponse
                        {
                            Id = ou2.Id,
                            Code = ou2.Code,
                            Name = ou2.Name
                        } : null,
                        Note = js.Note,
                        Order = js.Order,
                    },
                    PlanId = js.MaintenancePlanDefinitionId
                }
            ).ToListAsync(cancellationToken);

            // Query 5: Tất cả RequiredItems cho các MaintenancePlans
            var allRequiredItems = await (
                from ri in _qq.Query<MaintenancePlanRequiredItem>()
                join ig in _qq.Query<ItemGroup>() on ri.ItemGroupId equals ig.Id into igGroup
                from ig in igGroup.DefaultIfEmpty()
                join i in _qq.Query<Item>() on ri.ItemId equals i.Id into iGroup
                from i in iGroup.DefaultIfEmpty()
                join u in _qq.Query<UnitOfMeasure>() on ri.UnitOfMeasureId equals u.Id into uGroup
                from u in uGroup.DefaultIfEmpty()
                where planIds.Contains(ri.MaintenancePlanDefinitionId)
                select new
                {
                    RequiredItem = new MaintenancePlanRequiredItemResponse
                    {
                        Id = ri.Id,
                        MaintenancePlanDefinitionId = ri.MaintenancePlanDefinitionId,
                        ItemGroupId = ri.ItemGroupId,
                        ItemGroupName = ig.Name,
                        ItemId = ri.ItemId,
                        ItemName = i.Name,
                        UnitOfMeasureId = ri.UnitOfMeasureId,
                        UnitOfMeasureName = u.Name,
                        Quantity = ri.Quantity,
                        IsRequired = ri.IsRequired,
                        Note = ri.Note
                    },
                    PlanId = ri.MaintenancePlanDefinitionId
                }
            ).ToListAsync(cancellationToken);

            // Assemble data in memory
            foreach (var plan in maintenancePlans)
            {
                plan.JobSteps = (
                    from js in allJobSteps
                    where js.PlanId == plan.Id
                    select js.JobStep
                ).ToList();

                plan.RequiredItems = (
                    from ri in allRequiredItems
                    where ri.PlanId == plan.Id
                    select ri.RequiredItem
                ).ToList();
            }
        }

        asset.MaintenancePlanDefinitions = maintenancePlans;

        return Result<AssetResponse>.Success(asset);
    }
}
