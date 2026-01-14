using System.Reflection.Metadata;
using Emm.Domain.Abstractions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.Inventory;

public class GoodsIssue : AggregateRoot, IAuditableEntity
{
    public NaturalKey Code { get; private set; }
    public Guid WarehouseId { get; private set; }
    public DocumentStatus Status { get; private set; } = DocumentStatus.Idle;
    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;
    public string? Notes { get; private set; }

    private readonly List<GoodsIssueLine> _lines = [];
    public IReadOnlyCollection<GoodsIssueLine> Lines => _lines.AsReadOnly();

    public GoodsIssue(
        NaturalKey code,
        Guid warehouseId,
        DocumentStatus status,
        string? notes = null)
    {
        Code = code;
        WarehouseId = warehouseId;
        Status = status;
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
        var line = new GoodsIssueLine(
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


    private GoodsIssue() { } // EF Core constructor
}

public class GoodsIssueLine
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid GoodsIssueId { get; private set; }
    public Guid ItemId { get; private set; }
    public string ItemCode { get; private set; } = null!;
    public string ItemName { get; private set; } = null!;
    public Guid UnitOfMeasureId { get; private set; }
    public string UnitOfMeasureCode { get; private set; } = null!;
    public string UnitOfMeasureName { get; private set; } = null!;
    public decimal Quantity { get; private set; }

    public GoodsIssueLine(
        Guid goodsIssueId,
        Guid itemId,
        string itemCode,
        string itemName,
        Guid unitOfMeasureId,
        string unitOfMeasureCode,
        string unitOfMeasureName,
        decimal quantity)
    {
        GoodsIssueId = goodsIssueId;
        ItemId = itemId;
        ItemCode = itemCode;
        ItemName = itemName;
        UnitOfMeasureId = unitOfMeasureId;
        UnitOfMeasureCode = unitOfMeasureCode;
        UnitOfMeasureName = unitOfMeasureName;
        Quantity = quantity;
    }

    private GoodsIssueLine() { } // EF Core constructor
}
