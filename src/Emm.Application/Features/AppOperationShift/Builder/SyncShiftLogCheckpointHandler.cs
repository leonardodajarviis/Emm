using Emm.Domain.Entities.Inventory;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Builder;

public class SyncShiftLogCheckpointHandler : IUpdateShiftLogBuilderHandler
{
    private readonly IQueryContext _queryContext;

    public SyncShiftLogCheckpointHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result> Handle(UpdateShiftLogContext context, CancellationToken cancellationToken)
    {
        var requestCheckpointIds = context.Data.Checkpoints
            .Where(c => c.Id.HasValue)
            .Select(c => c.Id!.Value)
            .ToHashSet();

        var shiftLog = context.ShiftLog;

        CollectionHelper.RemoveItemsNotInRequest(
            context.ShiftLog.Checkpoints,
            requestCheckpointIds,
            c => c.Id,
            context.ShiftLog.RemoveCheckpoint);

        var locationIds = context.Data.Checkpoints
            .Select(c => c.LocationId)
            .Distinct()
            .ToArray();

        var localDict = await _queryContext.Query<Location>()
            .Where(l => locationIds.Contains(l.Id))
            .ToDictionaryAsync(l => l.Id, l => l, cancellationToken);

        var itemIds = context.Data.Checkpoints
            .Where(c => c.ItemId.HasValue)
            .Select(c => c.ItemId!.Value)
            .Distinct()
            .ToArray();

        var itemDic = await _queryContext.Query<Item>()
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, i => i, cancellationToken);

        foreach (var cp in context.Data.Checkpoints)
        {
            if (cp.Id.HasValue)
            {
                if (cp.ItemId != null && cp.IsWithAttachedMaterial)
                {
                    var item = itemDic.GetValueOrDefault(cp.ItemId.Value);
                    if (item == null)
                    {
                        return Result.Validation($"Item with ID {cp.ItemId.Value} does not exist");
                    }

                    context.ShiftLog.MakeAttchedMaterialInCheckpoint(
                        cp.Id.Value,
                        cp.ItemId.Value,
                        item.Code,
                        item.Name);
                }
                var location = localDict.GetValueOrDefault(cp.LocationId);
                if (location == null)
                {
                    return Result.Validation($"Location with ID {cp.LocationId} does not exist");
                }
                shiftLog.UpdateLocationInCheckpoint(cp.Id.Value, location.Id, location.Name);
            }
            else
            {
                var location = localDict.GetValueOrDefault(cp.LocationId);
                if (location == null)
                {
                    return Result.Validation($"Location with ID {cp.LocationId} does not exist");
                }

                context.ShiftLog.AddCheckpoint(cp.LinkedId, cp.Name, location.Id, location.Name);

                if (cp.ItemId != null && cp.IsWithAttachedMaterial)
                {
                    var item = itemDic.GetValueOrDefault(cp.ItemId.Value);
                    if (item == null)
                    {
                        return Result.Validation($"Item with ID {cp.ItemId.Value} does not exist");
                    }

                    context.ShiftLog.MakeAttchedMaterialInCheckpoint(
                        cp.LinkedId,
                        cp.ItemId.Value,
                        item.Code,
                        item.Name);
                }
            }
        }

        return Result.Success();
    }
}
