using Emm.Application.Abstractions;
using Emm.Application.Features.AppAssetModel.Dtos;
using Emm.Application.Features.AppParameterCatalog.Dtos;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Inventory;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAssetModel.Queries;

public class GetAssetModelByIdQueryHandler : IRequestHandler<GetAssetModelByIdQuery, Result<AssetModelDetailResponse>>
{
    private readonly IQueryContext _qq;
    private readonly IFileStorage _fileStorage;

    public GetAssetModelByIdQueryHandler(IQueryContext queryContext, IFileStorage fileStorage)
    {
        _qq = queryContext;
        _fileStorage = fileStorage;
    }

    public async Task<Result<AssetModelDetailResponse>> Handle(GetAssetModelByIdQuery request, CancellationToken cancellationToken)
    {
        var assetModel = await _qq.Query<AssetModel>()
            .Where(x => x.Id == request.Id)
            .Select(root => new AssetModelDetailResponse
            {
                Id = root.Id,
                Code = root.Code.Value,
                Name = root.Name,
                Description = root.Description,
                Notes = root.Notes,
                ParentId = root.ParentId,
                AssetCategoryId = root.AssetCategoryId,
                ThumbnailUrl = _fileStorage.GetFileUrl(root.ThumbnailUrl ?? ""),
                AssetTypeId = root.AssetTypeId,
                IsActive = root.IsActive,
                CreatedAt = root.Audit.CreatedAt,
                ModifiedAt = root.Audit.ModifiedAt,

                CreatedBy = _qq.Query<User>()
                    .Where(u => u.Id == root.Audit.CreatedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault() ?? "System",
                ModifiedBy = _qq.Query<User>()
                    .Where(u => u.Id == root.Audit.ModifiedByUserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefault(),

                ThumbnailFileId = root.ThumbnailFileId,

                Parameters = _qq.Query<AssetModelParameter>()
                    .Where(x => x.AssetModelId == root.Id)
                    .Join(_qq.Query<ParameterCatalog>(), mp => mp.ParameterId, p => p.Id, (mp, p) => new AssetParameterResponse
                    {
                        Id = mp.ParameterId,
                        Code = p.Code,
                        Name = p.Name,
                        Description = p.Description,
                        UnitOfMeasureId = p.UnitOfMeasureId,
                        IsMaintenanceParameter = mp.IsMaintenanceParameter,
                        UnitOfMeasureName = _qq.Query<UnitOfMeasure>()
                            .Where(uom => uom.Id == p.UnitOfMeasureId)
                            .Select(uom => uom.Name)
                            .FirstOrDefault()
                    })
                    .ToList(),

                Images = _qq.Query<AssetModelImage>()
                    .Where(x => x.AssetModelId == root.Id)
                    .Select(x => new AssetModelImageResponse
                    {
                        FileId = x.FileId,
                        FilePath = x.FilePath,
                        Url = _fileStorage.GetFileUrl(x.FilePath)
                    })
                    .ToList(),

                Parent = _qq.Query<AssetModel>()
                    .Where(x => x.Id == root.ParentId)
                    .Select(x => new AssetModelParentResponse
                    {
                        Id = x.Id,
                        Code = x.Code.Value,
                        Name = x.Name
                    })
                    .FirstOrDefault(),

                AssetCategory = _qq.Query<AssetCategory>()
                    .Where(x => x.Id == root.AssetCategoryId)
                    .Select(x => new AssetCategoryResponse
                    {
                        Id = x.Id,
                        Code = x.Code.ToString(),
                        Name = x.Name
                    }).FirstOrDefault(),

                AssetType = _qq.Query<AssetType>()
                    .Where(x => x.Id == root.AssetTypeId)
                    .Select(x => new AssetTypeResponse
                    {
                        Id = x.Id,
                        Code = x.Code.Value,
                        Name = x.Name
                    }).FirstOrDefault(),

                MaintenancePlanDefinitions = _qq.Query<MaintenancePlanDefinition>()
                    .Where(x => x.AssetModelId == root.Id)
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
                            TriggerValue = mp.ParameterBasedTrigger.Value,
                            PlusTolerance = mp.ParameterBasedTrigger.PlusTolerance,
                            MinusTolerance = mp.ParameterBasedTrigger.MinusTolerance,
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
                                    .Where(uom => uom.Id == ri.UnitOfMeasureId)
                                    .Select(uom => uom.Name)
                                    .FirstOrDefault(),
                                Quantity = ri.Quantity,
                                IsRequired = ri.IsRequired,
                                Note = ri.Note
                            })
                            .ToList()
                    })
                    .ToList(),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (assetModel == null)
        {
            return Result<AssetModelDetailResponse>.Failure(ErrorType.NotFound, "AssetModel not found.");
        }

        return Result<AssetModelDetailResponse>.Success(assetModel);
    }
}
