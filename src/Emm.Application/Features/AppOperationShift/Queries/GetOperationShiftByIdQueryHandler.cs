using System.Data.Common;
using Emm.Application.Features.AppOperationShift.Dtos;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Operations;
using Emm.Domain.Entities.Organization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Queries;

public class GetOperationShiftByIdQueryHandler : IRequestHandler<GetOperationShiftByIdQuery, Result<OperationShiftResponse>>
{
    private readonly IQueryContext _queryContext;

    public GetOperationShiftByIdQueryHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result<OperationShiftResponse>> Handle(GetOperationShiftByIdQuery request, CancellationToken cancellationToken)
    {
        // Query shift with its owned entities (Assets and Logs)
        var operationShift = await (
            from x in _queryContext.Query<OperationShift>()
            where x.Id == request.Id
            select new OperationShiftResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                OrganizationUnitId = x.OrganizationUnitId,
                PrimaryUserId = x.PrimaryUserId,
                IsCheckpointLogEnabled = x.IsCheckpointLogEnabled,
                PrimaryUserDisplayName = (
                    from e in _queryContext.Query<User>()
                    where e.Id == x.PrimaryUserId
                    select e.DisplayName
                ).FirstOrDefault(),
                ScheduledStartTime = x.ScheduledStartTime,
                ScheduledEndTime = x.ScheduledEndTime,
                ActualStartTime = x.ActualStartTime,
                ActualEndTime = x.ActualEndTime,
                Status = x.Status,
                Notes = x.Notes,
                CreatedAt = x.Audit.CreatedAt,
                ModifiedAt = x.Audit.ModifiedAt,

                AssetBoxes = (
                    from ab in x.AssetBoxes
                    select new OperationShiftAssetBoxResponse
                    {
                        Id = ab.Id,
                        OperationShiftId = ab.OperationShiftId,
                        BoxName = ab.BoxName,
                        Role = ab.Role,
                        Description = ab.Description,
                        DisplayOrder = ab.DisplayOrder,
                    }
                ).ToList(),

                Assets = (
                    from a in x.Assets
                    select new OperationShiftAssetResponse
                    {
                        Id = a.Id,
                        OperationShiftId = a.OperationShiftId,
                        AssetId = a.AssetId,
                        AssetBoxId = a.AssetBoxId,
                        AssetCode = a.AssetCode,
                        AssetName = a.AssetName,
                        IsPrimary = a.IsPrimary,
                        StartedAt = a.StartedAt,
                        CompletedAt = a.CompletedAt,
                        Notes = a.Notes,
                    }
                ).ToList(),
            }
        ).FirstOrDefaultAsync(cancellationToken);

        if (operationShift == null)
        {
            return Result<OperationShiftResponse>.Failure(ErrorType.NotFound, "Operation shift not found.");
        }

        // Query tasks separately (they are now a separate aggregate)
        var shiftLog = await (
            from t in _queryContext.Query<ShiftLog>()
            where t.OperationShiftId == request.Id
            select new ShiftLogResponse
            {
                Id = t.Id,
                OperationShiftId = t.OperationShiftId,
                Name = t.Name,
                BoxId = t.BoxId,
                AssetId = t.AssetId,
                Description = t.Description,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                Notes = t.Notes,

                Events = (
                    from e in t.Events
                    select new ShiftLogEventResponse
                    {
                        Id = e.Id,
                        ShiftLogId = e.ShiftLogId,
                        StartTime = e.StartTime,
                        EndTime = e.EndTime,
                        EventType = (int)e.EventType,
                    }
                ).ToList(),

                Checkpoints = (
                    from c in t.Checkpoints
                    select new ShiftLogCheckpointResponse
                    {
                        Id = c.Id,
                        OperationTaskId = c.ShiftLogId,
                        LinkedId = c.LinkedId,
                        LocationId = c.LocationId,
                        LocationName = c.LocationName,
                        IsWithAttachedMaterial = c.IsWithAttachedMaterial,
                        ItemId = c.ItemId,
                        ItemCode = c.ItemCode,
                        ItemName = c.ItemName
                    }
                ).ToList(),

                Readings = (
                    from r in t.Readings
                    join u in _queryContext.Query<UnitOfMeasure>()
                        on r.UnitOfMeasureId equals u.Id into uom
                    from u in uom.DefaultIfEmpty()
                    select new ShiftLogParameterReadingResponse
                    {
                        Id = r.Id,
                        AssetId = r.AssetId,
                        AssetCode = r.AssetCode,
                        AssetName = r.AssetName,
                        ShiftLogCheckpointLinkedId = r.ShiftLogCheckpointLinkedId,
                        OperationTaskId = r.ShiftLogId,
                        Value = r.Value,
                        Notes = r.Notes,
                        ParameterCode = r.ParameterCode,
                        ParameterId = r.ParameterId,
                        ParameterName = r.ParameterName,
                        ReadingTime = r.ReadingTime,
                        StringValue = r.StringValue,
                        UnitOfMeasureId = r.UnitOfMeasureId,
                        UnitOfMeasureName = u.Name,
                        UnitOfMeasureSymbol = u.Symbol
                    }
                ).ToList(),

                Items = (
                    from i in t.Items
                    select new ShiftLogItemResponse
                    {
                        Id = i.Id,
                        ShiftLogId = i.ShiftLogId,
                        ItemId = i.ItemId,
                        ItemName = i.ItemName,
                        Quantity = i.Quantity,
                        AssetId = i.AssetId,
                        AssetCode = i.AssetCode,
                        AssetName = i.AssetName,
                        UnitOfMeasureId = i.UnitOfMeasureId,
                        UnitOfMeasureName = i.UnitOfMeasureName
                    }
                ).ToList()
            }
        ).ToListAsync(cancellationToken);

        operationShift.ShiftLogs = shiftLog;


        var assetIds = (from a in operationShift.Assets select a.AssetId).ToArray();
        var assetParameters =
        await (
            from ap in _queryContext.Query<AssetParameter>()
            join u in _queryContext.Query<UnitOfMeasure>()
                on ap.UnitOfMeasureId equals u.Id into uom
            from u in uom.DefaultIfEmpty()
            where assetIds.Contains(ap.AssetId)
            select new OperationShiftAssetParameterResponse
            {
                AssetId = ap.AssetId,
                ParameterId = ap.ParameterId,
                ParameterCode = ap.ParameterCode,
                ParameterName = ap.ParameterName,
                UnitOfMeasureId = ap.UnitOfMeasureId,
                UnitOfMeasureName = u.Name,
                UnitOfMeasureSymbol = u.Symbol
            }
        ).ToListAsync(cancellationToken);

        foreach (var asset in operationShift.Assets)
        {
            asset.Parameters = [.. (from ap in assetParameters where ap.AssetId == asset.AssetId select ap)];
        }

        return Result<OperationShiftResponse>.Success(operationShift);
    }
}
