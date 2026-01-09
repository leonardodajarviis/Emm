using Emm.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Builder;

public class SyncShiftLogItemsHandler : IUpdateShiftLogBuilderHandler
{
    private readonly IQueryContext _queryContext;
    public SyncShiftLogItemsHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result> Handle(UpdateShiftLogContext context, CancellationToken cancellationToken)
    {
        var shiftLogItems = context.Data.Items;
        var shiftLog = context.ShiftLog;
        var requestItemIds = shiftLogItems
            .Where(i => i.Id.HasValue)
            .Select(i => i.Id!.Value)
            .ToHashSet();

        CollectionHelper.RemoveItemsNotInRequest(
            context.ShiftLog.Items,
            requestItemIds,
            i => i.Id,
            shiftLog.RemoveItem);

        var assets = context.AssetDict;
        var itemIds = shiftLogItems
            .Select(i => i.ItemId)
            .Distinct()
            .ToArray();

        var itemDict = await _queryContext.Query<Item>()
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, i => i, cancellationToken);

        foreach (var shiftLogItem in shiftLogItems)
        {
            if (shiftLogItem.Id.HasValue)
            {
                shiftLog.UpdateItemQuantity(shiftLogItem.Id.Value, shiftLogItem.Quantity);
            }
            else
            {
                // New item, add it
                var asset = assets.GetValueOrDefault(shiftLogItem.AssetId);
                var item = itemDict.GetValueOrDefault(shiftLogItem.ItemId);
                if (item == null)
                {
                    return Result.Validation($"Item with ID {shiftLogItem.ItemId} does not exist");
                }

                shiftLog.AddItem(
                    assetId: shiftLogItem.AssetId,
                    assetCode: asset?.AssetCode,
                    assetName: asset?.AssetName,
                    itemId: shiftLogItem.ItemId,
                    itemCode: item.Code,
                    itemName: item.Name,
                    quantity: shiftLogItem.Quantity,
                    warehouseIssueSlipId: shiftLogItem.WarehouseIssueSlipId);
            }
        }

        return Result.Success();
    }
}
