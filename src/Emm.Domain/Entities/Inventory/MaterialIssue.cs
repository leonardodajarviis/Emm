using Emm.Domain.Abstractions;
using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.Inventory;

/// <summary>
/// Aggregate Root đại diện cho Phiếu xuất vật tư.
/// Dùng để ghi nhận việc xuất vật tư từ kho dựa trên yêu cầu vật tư đã được duyệt.
/// </summary>
public class MaterialIssue : AggregateRoot, IAuditableEntity
{
    private const int MaxLinesPerIssue = 100;

    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public long? MaterialRequestId { get; private set; }
    public string? MaterialRequestCode { get; private set; }
    public long WarehouseId { get; private set; }
    public long OrganizationUnitId { get; private set; }
    public long IssuedByUserId { get; private set; }
    public long ReceivedByUserId { get; private set; }
    public MaterialIssueStatus Status { get; private set; }

    public DateTime IssueDate { get; private set; }
    public string? Remarks { get; private set; }

    public DateTime? ConfirmedAt { get; private set; }
    public long? ConfirmedByUserId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<MaterialIssueLine> _lines;
    public IReadOnlyCollection<MaterialIssueLine> Lines => _lines;

    private MaterialIssue()
    {
        _lines = [];
    }

    public MaterialIssue(
        string code,
        long warehouseId,
        long organizationUnitId,
        long issuedByUserId,
        long receivedByUserId,
        DateTime issueDate,
        long? materialRequestId = null,
        string? materialRequestCode = null,
        string? remarks = null)
    {
        ValidateCode(code);
        ValidateForeignKey(warehouseId, nameof(WarehouseId));
        ValidateForeignKey(organizationUnitId, nameof(OrganizationUnitId));
        ValidateForeignKey(issuedByUserId, nameof(IssuedByUserId));
        ValidateForeignKey(receivedByUserId, nameof(ReceivedByUserId));

        _lines = [];

        Code = code;
        WarehouseId = warehouseId;
        OrganizationUnitId = organizationUnitId;
        IssuedByUserId = issuedByUserId;
        ReceivedByUserId = receivedByUserId;
        IssueDate = issueDate;
        MaterialRequestId = materialRequestId;
        MaterialRequestCode = materialRequestCode;
        Remarks = remarks;
        Status = MaterialIssueStatus.Draft;

        // RaiseDomainEvent(new MaterialIssueCreatedEvent(code, warehouseId, organizationUnitId));
    }

    #region Line Management

    public void AddLine(
        long itemId,
        string itemCode,
        string itemName,
        decimal quantity,
        decimal unitCost,
        string? unit = null,
        string? lotNumber = null,
        string? serialNumber = null,
        long? locationId = null,
        string? remarks = null)
    {
        if (Status != MaterialIssueStatus.Draft)
            throw new DomainException("Can only add lines to draft issues");

        if (_lines.Count >= MaxLinesPerIssue)
            throw new DomainException($"Cannot exceed {MaxLinesPerIssue} lines per issue");

        var lineNumber = _lines.Count + 1;
        var line = new MaterialIssueLine(
            lineNumber,
            itemId,
            itemCode,
            itemName,
            quantity,
            unitCost,
            unit,
            lotNumber,
            serialNumber,
            locationId,
            remarks);

        _lines.Add(line);

        // RaiseDomainEvent(new MaterialIssueLineAddedEvent(Id, itemId, quantity));
    }

    public void UpdateLineQuantity(long itemId, decimal newQuantity)
    {
        if (Status != MaterialIssueStatus.Draft)
            throw new DomainException("Can only update lines in draft issues");

        var line = _lines.FirstOrDefault(l => l.ItemId == itemId)
            ?? throw new DomainException($"Item with Id {itemId} not found in this issue");

        line.UpdateQuantity(newQuantity);

        // RaiseDomainEvent(new MaterialIssueLineUpdatedEvent(Id, itemId, newQuantity));
    }

    public void RemoveLine(long itemId)
    {
        if (Status != MaterialIssueStatus.Draft)
            throw new DomainException("Can only remove lines from draft issues");

        var line = _lines.FirstOrDefault(l => l.ItemId == itemId)
            ?? throw new DomainException($"Item with Id {itemId} not found in this issue");

        _lines.Remove(line);
        ReorderLines();

        // RaiseDomainEvent(new MaterialIssueLineRemovedEvent(Id, itemId));
    }

    private void ReorderLines()
    {
        var lineNumber = 1;
        foreach (var line in _lines.OrderBy(l => l.LineNumber))
        {
            line.SetLineNumber(lineNumber++);
        }
    }

    #endregion

    #region Status Management

    public void Submit()
    {
        if (Status != MaterialIssueStatus.Draft)
            throw new DomainException("Only draft issues can be submitted");

        if (!_lines.Any())
            throw new DomainException("Cannot submit an issue without any lines");

        Status = MaterialIssueStatus.Pending;

        // RaiseDomainEvent(new MaterialIssueSubmittedEvent(Id, Code));
    }

    public void Confirm(long confirmedByUserId)
    {
        if (Status != MaterialIssueStatus.Pending)
            throw new DomainException("Only pending issues can be confirmed");

        ValidateForeignKey(confirmedByUserId, nameof(confirmedByUserId));

        Status = MaterialIssueStatus.Confirmed;
        ConfirmedByUserId = confirmedByUserId;
        ConfirmedAt = DateTime.UtcNow;

        // RaiseDomainEvent(new MaterialIssueConfirmedEvent(Id, Code, confirmedByUserId));
    }

    public void Complete()
    {
        if (Status != MaterialIssueStatus.Confirmed)
            throw new DomainException("Only confirmed issues can be completed");

        Status = MaterialIssueStatus.Completed;

        // RaiseDomainEvent(new MaterialIssueCompletedEvent(Id, Code, TotalCost));
    }

    public void Cancel(string reason)
    {
        if (Status == MaterialIssueStatus.Completed || Status == MaterialIssueStatus.Cancelled)
            throw new DomainException("Cannot cancel a completed or already cancelled issue");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Cancellation reason is required");

        Status = MaterialIssueStatus.Cancelled;
        Remarks = $"{Remarks}\nCancellation reason: {reason}".Trim();

        // RaiseDomainEvent(new MaterialIssueCancelledEvent(Id, Code, reason));
    }

    #endregion

    #region Computed Properties

    public decimal TotalQuantity => _lines.Sum(l => l.Quantity);

    public decimal TotalCost => _lines.Sum(l => l.TotalCost);

    public int TotalLineCount => _lines.Count;

    #endregion

    #region Validation Methods

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Material issue code cannot be empty");

        if (code.Length > 50)
            throw new DomainException("Material issue code cannot exceed 50 characters");
    }

    private static void ValidateForeignKey(long foreignKeyId, string fieldName)
    {
        if (foreignKeyId <= 0)
            throw new DomainException($"{fieldName} must be greater than zero");
    }

    #endregion
}

#region Enums

public enum MaterialIssueStatus
{
    Draft = 0,
    Pending = 1,
    Confirmed = 2,
    Completed = 3,
    Cancelled = 4
}

#endregion

#region Child Entities

/// <summary>
/// Entity đại diện cho một dòng trong phiếu xuất vật tư.
/// </summary>
public class MaterialIssueLine
{
    public long Id { get; private set; }
    public int LineNumber { get; private set; }
    public long ItemId { get; private set; }
    public string ItemCode { get; private set; } = null!;
    public string ItemName { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public string? Unit { get; private set; }
    public string? LotNumber { get; private set; }
    public string? SerialNumber { get; private set; }
    public long? LocationId { get; private set; }
    public string? Remarks { get; private set; }

    public decimal TotalCost => Quantity * UnitCost;

    private MaterialIssueLine() { }

    public MaterialIssueLine(
        int lineNumber,
        long itemId,
        string itemCode,
        string itemName,
        decimal quantity,
        decimal unitCost,
        string? unit = null,
        string? lotNumber = null,
        string? serialNumber = null,
        long? locationId = null,
        string? remarks = null)
    {
        if (itemId <= 0)
            throw new DomainException("ItemId must be greater than zero");

        if (string.IsNullOrWhiteSpace(itemCode))
            throw new DomainException("Item code cannot be empty");

        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainException("Item name cannot be empty");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        if (unitCost < 0)
            throw new DomainException("Unit cost cannot be negative");

        LineNumber = lineNumber;
        ItemId = itemId;
        ItemCode = itemCode;
        ItemName = itemName;
        Quantity = quantity;
        UnitCost = unitCost;
        Unit = unit;
        LotNumber = lotNumber;
        SerialNumber = serialNumber;
        LocationId = locationId;
        Remarks = remarks;
    }

    public void UpdateQuantity(decimal newQuantity)
    {
        if (newQuantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        Quantity = newQuantity;
    }

    public void UpdateUnitCost(decimal newUnitCost)
    {
        if (newUnitCost < 0)
            throw new DomainException("Unit cost cannot be negative");

        UnitCost = newUnitCost;
    }

    internal void SetLineNumber(int lineNumber)
    {
        LineNumber = lineNumber;
    }
}

#endregion
