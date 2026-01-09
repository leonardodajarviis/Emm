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

        foreach (var item in context.Data.Items)
        {
            var asset = context.AssetDict.GetValueOrDefault(item.AssetId);
            if (asset == null)
            {
                return Result.Validation($"Asset with ID {item.AssetId} does not exist");
            }
            var itemInfo = itemDict.GetValueOrDefault(item.ItemId);
            if (itemInfo == null)
            {
                return Result.Validation($"Item with ID {item.ItemId} does not exist");
            }

            shiftLog.AddItem(
                itemId: item.ItemId,
                warehouseIssueSlipId: item.WarehouseIssueSlipId,
                assetCode: asset.AssetCode,
                assetName: asset.AssetName,
                itemCode: itemInfo.Code,
                itemName: itemInfo.Name,
                quantity: item.Quantity,
                unitOfMeasureId: itemInfo.UnitOfMeasureId);
        }

        return Result.Success();
    }
}
