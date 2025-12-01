using Emm.Application.Features.AppAsset.Dtos;
using Emm.Application.Features.AppAssetModel.Dtos;
using Emm.Domain.Entities.AssetCatalog;
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
                Code = x.Code,
                DisplayName = x.DisplayName,
                AssetCategoryId = x.AssetCategoryId,
                AssetCategoryCode = x.AssetCategoryCode,
                AssetCategoryName = x.AssetCategoryName,
                AssetModelId = x.AssetModelId,
                AssetModelCode = x.AssetModelCode,
                AssetModelName = x.AssetModelName,
                AssetTypeId = x.AssetTypeId,
                AssetTypeCode = x.AssetTypeCode,
                AssetTypeName = x.AssetTypeName,
                Description = x.Description,
                Status = (int)x.Status,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,

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
                        CreatedAt = mp.CreatedAt,
                        UpdatedAt = mp.UpdatedAt,

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
                                ItemId = ri.ItemId,
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
