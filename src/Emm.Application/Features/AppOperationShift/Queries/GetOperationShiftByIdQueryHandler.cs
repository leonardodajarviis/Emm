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
        var operationShift = await _queryContext.Query<OperationShift>()
            .Where(x => x.Id == request.Id)
            .Select(x => new OperationShiftResponse
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                OrganizationUnitId = x.OrganizationUnitId,
                PrimaryUserId = x.PrimaryUserId,
                IsCheckpointLogEnabled = x.IsCheckpointLogEnabled,
                PrimaryUserDisplayName = _queryContext.Query<User>()
                    .Where(e => e.Id == x.PrimaryUserId)
                    .Select(e => e.DisplayName)
                    .FirstOrDefault(),
                ScheduledStartTime = x.ScheduledStartTime,
                ScheduledEndTime = x.ScheduledEndTime,
                ActualStartTime = x.ActualStartTime,
                ActualEndTime = x.ActualEndTime,
                Status = x.Status,
                Notes = x.Notes,
                CreatedAt = x.Audit.CreatedAt,
                ModifiedAt = x.Audit.ModifiedAt,

                Assets = x.Assets.Select(a => new OperationShiftAssetResponse
                {
                    Id = a.Id,
                    OperationShiftId = a.OperationShiftId,
                    AssetId = a.AssetId,
                    AssetCode = a.AssetCode,
                    AssetName = a.AssetName,
                    IsPrimary = a.IsPrimary,
                    StartedAt = a.StartedAt,
                    CompletedAt = a.CompletedAt,
                    Notes = a.Notes,
                }).ToList(),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (operationShift == null)
        {
            return Result<OperationShiftResponse>.Failure(ErrorType.NotFound, "Operation shift not found.");
        }

        // Query tasks separately (they are now a separate aggregate)
        var shiftLog = await _queryContext.Query<ShiftLog>()
            .Where(t => t.OperationShiftId == request.Id)
            .Select(t => new ShiftLogResponse
            {
                Id = t.Id,
                OperationShiftId = t.OperationShiftId,
                Name = t.Name,
                Description = t.Description,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                Notes = t.Notes,

                Checkpoints = t.Checkpoints.Select(c => new ShiftLogCheckpointResponse
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
                }).ToList(),

                Readings = t.Readings.Select(r => new ShiftLogParameterReadingResponse
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
                    Unit = r.Unit
                }).ToList(),

                Items = t.Items.Select(i => new ShiftLogItemResponse
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
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        operationShift.ShiftLogs = shiftLog;


        var assetIds = operationShift.Assets.Select(a => a.AssetId).ToArray();
        var assetParameters = await _queryContext.Query<AssetParameter>()
            .Where(ap => assetIds.Contains(ap.AssetId))
            .Select(ap => new OperationShiftAssetParameterResponse
            {
                AssetId = ap.AssetId,
                ParameterId = ap.ParameterId,
                ParameterCode = ap.ParameterCode,
                ParameterName = ap.ParameterName,
                ParameterUnit = ap.ParameterUnit
            }).ToListAsync(cancellationToken);

        foreach (var asset in operationShift.Assets)
        {
            asset.Parameters = [.. assetParameters.Where(ap => ap.AssetId == asset.AssetId)];
        }

        return Result<OperationShiftResponse>.Success(operationShift);
    }
}
