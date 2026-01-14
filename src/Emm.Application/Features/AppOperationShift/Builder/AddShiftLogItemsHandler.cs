using Emm.Domain.Entities;
using Emm.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Builder;

public class AddShiftLogItemsHandler : ICreateShiftLogBuilderHandler
{
    private readonly IQueryContext _queryContext;
    public AddShiftLogItemsHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result> Handle(CreateShiftLogContext context, CancellationToken cancellationToken)
    {
        var itemIds = context.Data.Items.Select(i => i.ItemId).Distinct().ToArray();
        var shiftLog = context.ShiftLog;

        var itemDict = await _queryContext.Query<Item>()
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, i => i, cancellationToken);

        var unitIds = context.Data.Items
            .Where(i => i.UnitOfMeasureId.HasValue)
            .Select(i => i.UnitOfMeasureId!.Value)
            .Distinct()
            .ToArray();

        var unitDict = await _queryContext.Query<UnitOfMeasure>()
            .Where(u => unitIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u, cancellationToken);

        var unitCategoryIds = itemDict.Values
            .Select(i => i.UnitOfMeasureCategoryId)
            .Distinct()
            .ToArray();

        var unitCategoryDict = await _queryContext.Query<UnitOfMeasureCategory>()
            .Include(uc => uc.Lines)
            .Where(uc => unitCategoryIds.Contains(uc.Id))
            .ToDictionaryAsync(uc => uc.Id, uc => uc, cancellationToken);

        foreach (var shiftLogItem in context.Data.Items)
        {
            var asset = context.AssetDict.GetValueOrDefault(shiftLogItem.AssetId);
            if (asset == null)
            {
                return Result.Invalid($"Tài sản với ID {shiftLogItem.AssetId} không tồn tại");
            }

            var item = itemDict.GetValueOrDefault(shiftLogItem.ItemId);
            if (item == null)
            {
                return Result.Invalid($"Vật tư với ID {shiftLogItem.ItemId} không tồn tại");
            }

            if (item.UnitOfMeasureCategoryId is null)
            {
                return Result.Invalid($"Vật tư với ID {shiftLogItem.ItemId} không có nhóm đơn vị tính");
            }

            var existUnit = unitCategoryDict.GetValueOrDefault(item.UnitOfMeasureCategoryId ?? Guid.Empty)?.Lines
                .Any(l => l.UnitOfMeasureId == shiftLogItem.UnitOfMeasureId);

            if (existUnit != true)
            {
                return Result.Invalid($"Đơn vị tính cho vật tư với ID {shiftLogItem.ItemId} không tồn tại trong nhóm đơn vị tính của thiết bị");
            }

            var unit = unitDict.GetValueOrDefault(shiftLogItem.UnitOfMeasureId!.Value);

            shiftLog.AddItem(
                itemId: shiftLogItem.ItemId,
                warehouseIssueSlipId: shiftLogItem.WarehouseIssueSlipId,
                assetId: shiftLogItem.AssetId,
                assetCode: asset.AssetCode,
                assetName: asset.AssetName,
                itemCode: item.Code,
                itemName: item.Name,
                quantity: shiftLogItem.Quantity,
                unitOfMeasureId: shiftLogItem.UnitOfMeasureId,
                unitOfMeasureCode: unit?.Code,
                unitOfMeasureName: unit?.Name);
        }

        return Result.Success();
    }
}
