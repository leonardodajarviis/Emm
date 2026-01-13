using Emm.Domain.Abstractions;
using Emm.Domain.Exceptions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.Inventory;

/// <summary>
/// Aggregate Root đại diện cho Phiếu trả lại vật tư.
/// Dùng để ghi nhận việc nhận lại vật tư vào kho dựa trên yêu cầu trả lại đã được duyệt.
/// </summary>
public class MaterialReturn : AggregateRoot, IAuditableEntity
{
    private const int MaxLinesPerReturn = 100;

    public string Code { get; private set; } = null!;
    public Guid? MaterialReturnRequestId { get; private set; }
    public string? MaterialReturnRequestCode { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid OrganizationUnitId { get; private set; }
    public Guid ReturnedByUserId { get; private set; }
    public Guid ReceivedByUserId { get; private set; }
    public MaterialReturnStatus Status { get; private set; }

    public DateTime ReturnDate { get; private set; }
    public string? Remarks { get; private set; }

    public DateTime? InspectedAt { get; private set; }
    public Guid? InspectedByUserId { get; private set; }
    public string? InspectionNotes { get; private set; }

    public DateTime? ConfirmedAt { get; private set; }
    public Guid? ConfirmedByUserId { get; private set; }

    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    private readonly List<MaterialReturnLine> _lines;
    public IReadOnlyCollection<MaterialReturnLine> Lines => _lines;

    private MaterialReturn()
    {
        _lines = [];
    }

    public MaterialReturn(
        string code,
        Guid warehouseId,
        Guid organizationUnitId,
        Guid returnedByUserId,
        Guid receivedByUserId,
        DateTime returnDate,
        Guid? materialReturnRequestId = null,
        string? materialReturnRequestCode = null,
        string? remarks = null)
    {
        ValidateCode(code);

        _lines = [];

        Code = code;
        WarehouseId = warehouseId;
        OrganizationUnitId = organizationUnitId;
        ReturnedByUserId = returnedByUserId;
        ReceivedByUserId = receivedByUserId;
        ReturnDate = returnDate;
        MaterialReturnRequestId = materialReturnRequestId;
        MaterialReturnRequestCode = materialReturnRequestCode;
        Remarks = remarks;
        Status = MaterialReturnStatus.Draft;

        // RaiseDomainEvent(new MaterialReturnCreatedEvent(code, warehouseId, organizationUnitId));
    }

    #region Line Management

    public void AddLine(
        Guid itemId,
        string itemCode,
        string itemName,
        decimal quantity,
        decimal unitCost,
        string? unit = null,
        string? lotNumber = null,
        string? serialNumber = null,
        Guid? locationId = null,
        MaterialCondition condition = MaterialCondition.Good,
        string? remarks = null)
    {
        if (Status != MaterialReturnStatus.Draft)
            throw new DomainException("Can only add lines to draft returns");

        if (_lines.Count >= MaxLinesPerReturn)
            throw new DomainException($"Cannot exceed {MaxLinesPerReturn} lines per return");

        var lineNumber = _lines.Count + 1;
        var line = new MaterialReturnLine(
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
            condition,
            remarks);

        _lines.Add(line);

        // RaiseDomainEvent(new MaterialReturnLineAddedEvent(Id, itemId, quantity));
    }

    public void UpdateLineQuantity(int lineNumber, decimal newQuantity)
    {
        if (Status != MaterialReturnStatus.Draft)
            throw new DomainException("Can only update lines in draft returns");

        var line = _lines.FirstOrDefault(l => l.LineNumber == lineNumber)
            ?? throw new DomainException($"Line number {lineNumber} not found in this return");

        line.UpdateQuantity(newQuantity);

        // RaiseDomainEvent(new MaterialReturnLineUpdatedEvent(Id, line.ItemId, newQuantity));
    }

    public void UpdateLineCondition(int lineNumber, MaterialCondition newCondition, string? inspectionRemarks = null)
    {
        if (Status != MaterialReturnStatus.Draft && Status != MaterialReturnStatus.PendingInspection)
            throw new DomainException("Can only update condition in draft or pending inspection returns");

        var line = _lines.FirstOrDefault(l => l.LineNumber == lineNumber)
            ?? throw new DomainException($"Line number {lineNumber} not found in this return");

        line.UpdateCondition(newCondition, inspectionRemarks);
    }

    public void RemoveLine(int lineNumber)
    {
        if (Status != MaterialReturnStatus.Draft)
            throw new DomainException("Can only remove lines from draft returns");

        var line = _lines.FirstOrDefault(l => l.LineNumber == lineNumber)
            ?? throw new DomainException($"Line number {lineNumber} not found in this return");

        _lines.Remove(line);
        ReorderLines();

        // RaiseDomainEvent(new MaterialReturnLineRemovedEvent(Id, line.ItemId));
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
        if (Status != MaterialReturnStatus.Draft)
            throw new DomainException("Only draft returns can be submitted");

        if (!_lines.Any())
            throw new DomainException("Cannot submit a return without any lines");

        Status = MaterialReturnStatus.PendingInspection;

        // RaiseDomainEvent(new MaterialReturnSubmittedEvent(Id, Code));
    }

    public void Inspect(Guid inspectedByUserId, string? notes = null)
    {
        if (Status != MaterialReturnStatus.PendingInspection)
            throw new DomainException("Only pending inspection returns can be inspected");

        Status = MaterialReturnStatus.Inspected;
        InspectedByUserId = inspectedByUserId;
        InspectedAt = DateTime.UtcNow;
        InspectionNotes = notes;

        // Set accepted quantity based on condition
        foreach (var line in _lines)
        {
            if (line.Condition == MaterialCondition.Good)
            {
                line.SetAcceptedQuantity(line.Quantity);
            }
            else
            {
                line.SetRejectedQuantity(line.Quantity);
            }
        }

        // RaiseDomainEvent(new MaterialReturnInspectedEvent(Id, Code, inspectedByUserId));
    }

    public void InspectWithDetails(Guid inspectedByUserId, Dictionary<int, (decimal acceptedQty, decimal rejectedQty)> lineInspections, string? notes = null)
    {
        if (Status != MaterialReturnStatus.PendingInspection)
            throw new DomainException("Only pending inspection returns can be inspected");

        Status = MaterialReturnStatus.Inspected;
        InspectedByUserId = inspectedByUserId;
        InspectedAt = DateTime.UtcNow;
        InspectionNotes = notes;

        foreach (var (lineNumber, (acceptedQty, rejectedQty)) in lineInspections)
        {
            var line = _lines.FirstOrDefault(l => l.LineNumber == lineNumber)
                ?? throw new DomainException($"Line number {lineNumber} not found");

            line.SetAcceptedQuantity(acceptedQty);
            line.SetRejectedQuantity(rejectedQty);
        }

        // RaiseDomainEvent(new MaterialReturnInspectedEvent(Id, Code, inspectedByUserId));
    }

    public void Confirm(Guid confirmedByUserId)
    {
        if (Status != MaterialReturnStatus.Inspected)
            throw new DomainException("Only inspected returns can be confirmed");

        Status = MaterialReturnStatus.Confirmed;
        ConfirmedByUserId = confirmedByUserId;
        ConfirmedAt = DateTime.UtcNow;

        // RaiseDomainEvent(new MaterialReturnConfirmedEvent(Id, Code, confirmedByUserId));
    }

    public void Complete()
    {
        if (Status != MaterialReturnStatus.Confirmed)
            throw new DomainException("Only confirmed returns can be completed");

        Status = MaterialReturnStatus.Completed;

        // RaiseDomainEvent(new MaterialReturnCompletedEvent(Id, Code, TotalAcceptedValue));
    }

    public void Cancel(string reason)
    {
        if (Status == MaterialReturnStatus.Completed || Status == MaterialReturnStatus.Cancelled)
            throw new DomainException("Cannot cancel a completed or already cancelled return");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Cancellation reason is required");

        Status = MaterialReturnStatus.Cancelled;
        Remarks = $"{Remarks}\nCancellation reason: {reason}".Trim();

        // RaiseDomainEvent(new MaterialReturnCancelledEvent(Id, Code, reason));
    }

    #endregion

    #region Computed Properties

    public decimal TotalQuantity => _lines.Sum(l => l.Quantity);

    public decimal TotalAcceptedQuantity => _lines.Sum(l => l.AcceptedQuantity);

    public decimal TotalRejectedQuantity => _lines.Sum(l => l.RejectedQuantity);

    public decimal TotalValue => _lines.Sum(l => l.TotalCost);

    public decimal TotalAcceptedValue => _lines.Sum(l => l.AcceptedQuantity * l.UnitCost);

    public decimal TotalRejectedValue => _lines.Sum(l => l.RejectedQuantity * l.UnitCost);

    public int TotalLineCount => _lines.Count;

    public int GoodConditionLineCount => _lines.Count(l => l.Condition == MaterialCondition.Good);

    public int DamagedLineCount => _lines.Count(l => l.Condition != MaterialCondition.Good);

    public decimal AcceptanceRate => TotalQuantity == 0 ? 0 : Math.Round(TotalAcceptedQuantity / TotalQuantity * 100, 2);

    #endregion

    #region Validation Methods

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Material return code cannot be empty");

        if (code.Length > 50)
            throw new DomainException("Material return code cannot exceed 50 characters");
    }

    #endregion
}

#region Enums

public enum MaterialReturnStatus
{
    Draft = 0,
    PendingInspection = 1,
    Inspected = 2,
    Confirmed = 3,
    Completed = 4,
    Cancelled = 5
}

#endregion

#region Child Entities

/// <summary>
/// Entity đại diện cho một dòng trong phiếu trả lại vật tư.
/// </summary>
public class MaterialReturnLine
{
    public Guid Id { get; private set; }
    public int LineNumber { get; private set; }
    public Guid ItemId { get; private set; }
    public string ItemCode { get; private set; } = null!;
    public string ItemName { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public decimal AcceptedQuantity { get; private set; }
    public decimal RejectedQuantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public string? Unit { get; private set; }
    public string? LotNumber { get; private set; }
    public string? SerialNumber { get; private set; }
    public Guid? LocationId { get; private set; }
    public MaterialCondition Condition { get; private set; }
    public string? Remarks { get; private set; }
    public string? InspectionRemarks { get; private set; }

    public decimal TotalCost => Quantity * UnitCost;

    private MaterialReturnLine() { }

    public MaterialReturnLine(
        int lineNumber,
        Guid itemId,
        string itemCode,
        string itemName,
        decimal quantity,
        decimal unitCost,
        string? unit = null,
        string? lotNumber = null,
        string? serialNumber = null,
        Guid? locationId = null,
        MaterialCondition condition = MaterialCondition.Good,
        string? remarks = null)
    {
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
        AcceptedQuantity = 0;
        RejectedQuantity = 0;
        UnitCost = unitCost;
        Unit = unit;
        LotNumber = lotNumber;
        SerialNumber = serialNumber;
        LocationId = locationId;
        Condition = condition;
        Remarks = remarks;
    }

    public void UpdateQuantity(decimal newQuantity)
    {
        if (newQuantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        Quantity = newQuantity;
    }

    public void UpdateCondition(MaterialCondition newCondition, string? inspectionRemarks = null)
    {
        Condition = newCondition;
        if (inspectionRemarks != null)
        {
            InspectionRemarks = inspectionRemarks;
        }
    }

    public void SetAcceptedQuantity(decimal quantity)
    {
        if (quantity < 0)
            throw new DomainException("Accepted quantity cannot be negative");

        if (quantity > Quantity)
            throw new DomainException("Accepted quantity cannot exceed total quantity");

        AcceptedQuantity = quantity;
    }

    public void SetRejectedQuantity(decimal quantity)
    {
        if (quantity < 0)
            throw new DomainException("Rejected quantity cannot be negative");

        if (quantity > Quantity)
            throw new DomainException("Rejected quantity cannot exceed total quantity");

        RejectedQuantity = quantity;
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
