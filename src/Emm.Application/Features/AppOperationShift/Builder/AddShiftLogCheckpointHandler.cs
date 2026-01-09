using Emm.Domain.Entities.Inventory;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Builder;

public class AddShiftLogCheckpointHandler : ICreateShiftLogBuilderHandler
{
    private readonly IQueryContext _queryContext;
    public AddShiftLogCheckpointHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }
    public async Task<Result> Handle(CreateShiftLogContext context, CancellationToken cancellationToken)
    {
        if (!context.Data.Checkpoints.Any())
        {
            return Result.Success();
        }

        var locationIds = context.Data.Checkpoints.Select(c => c.LocationId).Distinct().ToArray();

        var localDict = await _queryContext.Query<Location>()
            .Where(l => locationIds.Contains(l.Id))
            .ToDictionaryAsync(l => l.Id, l => l, cancellationToken);

        var itemIds = context.Data.Checkpoints
            .Where(c => c.ItemId.HasValue)
            .Select(c => c.ItemId!.Value)
            .Distinct()
            .ToArray();

        var itemDict = await _queryContext.Query<Item>()
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, i => i, cancellationToken);

        var checkpointRequests = context.Data.Checkpoints;
        var shiftLog = context.ShiftLog;

        foreach (var checkpoint in checkpointRequests)
        {
            var location = localDict.GetValueOrDefault(checkpoint.LocationId);
            if (location == null)
            {
                return Result.Validation($"Location with ID {checkpoint.LocationId} does not exist");
            }

            shiftLog.AddCheckpoint(
                linkedId: checkpoint.LinkedId,
                name: checkpoint.Name,
                locationId: checkpoint.LocationId,
                locationName: location.Name
            );

            if (checkpoint.IsWithAttachedMaterial && checkpoint.ItemId != null)
            {
                var item = itemDict.GetValueOrDefault(checkpoint.ItemId.Value);
                if (item == null)
                {
                    return Result.Validation($"Item with ID {checkpoint.ItemId.Value} does not exist");
                }

                shiftLog.MakeAttchedMaterialInCheckpoint(
                    checkpointId: shiftLog.Checkpoints.Last().Id,
                    itemId: item.Id,
                    itemCode: item.Code,
                    itemName: item.Name);
            }
        }

        return Result.Success();
    }
}
