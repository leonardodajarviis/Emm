using System.Reflection.Metadata;
using Emm.Domain.Abstractions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.Inventory;

public class WarehouseRequest : AggregateRoot, IAuditableEntity
{
    public string Code { get; private set; } = null!;
    public Guid WarehouseId { get; private set; }
    public Document Status { get; private set; }
    public AuditMetadata Audit { get; private set; } = null!;
    public Guid? AssetId { get; private set; }
    public string? AssetCode { get; private set; }
    public string? AssetName { get; private set; }
    public void SetAudit(AuditMetadata audit) => Audit = audit;
    public string? Notes { get; private set; }

    private readonly List<WarehouseRequestLine> _lines = [];
    public IReadOnlyCollection<WarehouseRequestLine> Lines => _lines.AsReadOnly();

    public WarehouseRequest(
        string code,
        Guid warehouseId,
        Document status,
        Guid? assetId = null,
        string? assetCode = null,
        string? assetName = null,
        string? notes = null)
    {
        Code = code;
        WarehouseId = warehouseId;
        Status = status;
        AssetId = assetId;
        AssetCode = assetCode;
        AssetName = assetName;
        Notes = notes;
    }

    public void AddLine(
        Guid itemId,
        string itemCode,
        string itemName,
        Guid unitOfMeasureId,
        string unitOfMeasureCode,
        string unitOfMeasureName,
        decimal quantity)
    {
        var line = new WarehouseRequestLine(
            Id,
            itemId,
            itemCode,
            itemName,
            unitOfMeasureId,
            unitOfMeasureCode,
            unitOfMeasureName,
            quantity);
        _lines.Add(line);
    }

    private WarehouseRequest() { } // EF Core constructor
}

public class WarehouseRequestLine
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid WarehouseRequestId { get; private set; }
    public Guid ItemId { get; private set; }
    public string ItemCode { get; private set; } = null!;
    public string ItemName { get; private set; } = null!;
    public Guid UnitOfMeasureId { get; private set; }
    public string UnitOfMeasureCode { get; private set; } = null!;
    public string UnitOfMeasureName { get; private set; } = null!;
    public decimal Quantity { get; private set; }

    public WarehouseRequestLine(
        Guid warehouseRequestId,
        Guid itemId,
        string itemCode,
        string itemName,
        Guid unitOfMeasureId,
        string unitOfMeasureCode,
        string unitOfMeasureName,
        decimal quantity)
    {
        WarehouseRequestId = warehouseRequestId;
        ItemId = itemId;
        ItemCode = itemCode;
        ItemName = itemName;
        UnitOfMeasureId = unitOfMeasureId;
        UnitOfMeasureCode = unitOfMeasureCode;
        UnitOfMeasureName = unitOfMeasureName;
        Quantity = quantity;
    }
}
