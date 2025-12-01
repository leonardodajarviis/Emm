using Emm.Domain.Abstractions;
using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.Inventory;

/// <summary>
/// Aggregate Root đại diện cho Phiếu yêu cầu trả lại vật tư.
/// Dùng để yêu cầu trả lại vật tư về kho (vật tư thừa, không sử dụng hết, v.v.).
/// </summary>
public class MaterialReturnRequest : AggregateRoot, IAuditableEntity
{
    private const int MaxLinesPerRequest = 100;

    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public long RequestedByUserId { get; private set; }
    public long OrganizationUnitId { get; private set; }
    public long? WarehouseId { get; private set; }
    public long? OriginalMaterialIssueId { get; private set; }
    public string? OriginalMaterialIssueCode { get; private set; }
    public long? MaintenanceTicketId { get; private set; }
    public MaterialReturnReason ReturnReason { get; private set; }
    public MaterialReturnRequestStatus Status { get; private set; }

    public DateTime RequestedDate { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public long? ApprovedByUserId { get; private set; }
    public string? ApprovalNotes { get; private set; }
    public string? RejectionReason { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<MaterialReturnRequestLine> _lines;
    public IReadOnlyCollection<MaterialReturnRequestLine> Lines => _lines;

    private MaterialReturnRequest()
    {
        _lines = [];
    }

    public MaterialReturnRequest(
        string code,
        string title,
        long requestedByUserId,
        long organizationUnitId,
        MaterialReturnReason returnReason,
        long? warehouseId = null,
        long? originalMaterialIssueId = null,
        string? originalMaterialIssueCode = null,
        long? maintenanceTicketId = null,
        string? description = null)
    {
        ValidateCode(code);
        ValidateTitle(title);
        ValidateForeignKey(requestedByUserId, nameof(RequestedByUserId));
        ValidateForeignKey(organizationUnitId, nameof(OrganizationUnitId));

        _lines = [];

        Code = code;
        Title = title;
        Description = description;
        RequestedByUserId = requestedByUserId;
        OrganizationUnitId = organizationUnitId;
        WarehouseId = warehouseId;
        OriginalMaterialIssueId = originalMaterialIssueId;
        OriginalMaterialIssueCode = originalMaterialIssueCode;
        MaintenanceTicketId = maintenanceTicketId;
        ReturnReason = returnReason;
        Status = MaterialReturnRequestStatus.Draft;
        RequestedDate = DateTime.UtcNow;

        // RaiseDomainEvent(new MaterialReturnRequestCreatedEvent(code, organizationUnitId, returnReason));
    }

    #region Line Management

    public void AddLine(
        long itemId,
        string itemCode,
        string itemName,
        decimal requestedQuantity,
        string? unit = null,
        string? lotNumber = null,
        string? serialNumber = null,
        MaterialCondition condition = MaterialCondition.Good,
        string? remarks = null)
    {
        if (Status != MaterialReturnRequestStatus.Draft)
            throw new DomainException("Can only add lines to draft requests");

        if (_lines.Count >= MaxLinesPerRequest)
            throw new DomainException($"Cannot exceed {MaxLinesPerRequest} lines per request");

        if (_lines.Any(l => l.ItemId == itemId && l.LotNumber == lotNumber && l.SerialNumber == serialNumber))
            throw new DomainException($"Item {itemCode} with the same lot/serial already exists in this request");

        var lineNumber = _lines.Count + 1;
        var line = new MaterialReturnRequestLine(
            lineNumber,
            itemId,
            itemCode,
            itemName,
            requestedQuantity,
            unit,
            lotNumber,
            serialNumber,
            condition,
            remarks);

        _lines.Add(line);

        // RaiseDomainEvent(new MaterialReturnRequestLineAddedEvent(Id, itemId, requestedQuantity));
    }

    public void UpdateLineQuantity(int lineNumber, decimal newQuantity)
    {
        if (Status != MaterialReturnRequestStatus.Draft)
            throw new DomainException("Can only update lines in draft requests");

        var line = _lines.FirstOrDefault(l => l.LineNumber == lineNumber)
            ?? throw new DomainException($"Line number {lineNumber} not found in this request");

        line.UpdateRequestedQuantity(newQuantity);

        // RaiseDomainEvent(new MaterialReturnRequestLineUpdatedEvent(Id, line.ItemId, newQuantity));
    }

    public void UpdateLineCondition(int lineNumber, MaterialCondition newCondition)
    {
        if (Status != MaterialReturnRequestStatus.Draft)
            throw new DomainException("Can only update lines in draft requests");

        var line = _lines.FirstOrDefault(l => l.LineNumber == lineNumber)
            ?? throw new DomainException($"Line number {lineNumber} not found in this request");

        line.UpdateCondition(newCondition);
    }

    public void RemoveLine(int lineNumber)
    {
        if (Status != MaterialReturnRequestStatus.Draft)
            throw new DomainException("Can only remove lines from draft requests");

        var line = _lines.FirstOrDefault(l => l.LineNumber == lineNumber)
            ?? throw new DomainException($"Line number {lineNumber} not found in this request");

        _lines.Remove(line);
        ReorderLines();

        // RaiseDomainEvent(new MaterialReturnRequestLineRemovedEvent(Id, line.ItemId));
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
        if (Status != MaterialReturnRequestStatus.Draft)
            throw new DomainException("Only draft requests can be submitted");

        if (!_lines.Any())
            throw new DomainException("Cannot submit a request without any lines");

        Status = MaterialReturnRequestStatus.Pending;

        // RaiseDomainEvent(new MaterialReturnRequestSubmittedEvent(Id, Code));
    }

    public void Approve(long approvedByUserId, string? notes = null)
    {
        if (Status != MaterialReturnRequestStatus.Pending)
            throw new DomainException("Only pending requests can be approved");

        ValidateForeignKey(approvedByUserId, nameof(approvedByUserId));

        Status = MaterialReturnRequestStatus.Approved;
        ApprovedByUserId = approvedByUserId;
        ApprovedAt = DateTime.UtcNow;
        ApprovalNotes = notes;

        // Approve all lines with their requested quantity
        foreach (var line in _lines)
        {
            line.SetApprovedQuantity(line.RequestedQuantity);
        }

        // RaiseDomainEvent(new MaterialReturnRequestApprovedEvent(Id, Code, approvedByUserId));
    }

    public void PartialApprove(long approvedByUserId, Dictionary<int, decimal> lineApprovals, string? notes = null)
    {
        if (Status != MaterialReturnRequestStatus.Pending)
            throw new DomainException("Only pending requests can be approved");

        ValidateForeignKey(approvedByUserId, nameof(approvedByUserId));

        Status = MaterialReturnRequestStatus.Approved;
        ApprovedByUserId = approvedByUserId;
        ApprovedAt = DateTime.UtcNow;
        ApprovalNotes = notes;

        foreach (var (lineNumber, approvedQty) in lineApprovals)
        {
            var line = _lines.FirstOrDefault(l => l.LineNumber == lineNumber)
                ?? throw new DomainException($"Line number {lineNumber} not found");

            line.SetApprovedQuantity(approvedQty);
        }

        // RaiseDomainEvent(new MaterialReturnRequestPartiallyApprovedEvent(Id, Code, approvedByUserId));
    }

    public void Reject(long rejectedByUserId, string reason)
    {
        if (Status != MaterialReturnRequestStatus.Pending)
            throw new DomainException("Only pending requests can be rejected");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Rejection reason is required");

        Status = MaterialReturnRequestStatus.Rejected;
        ApprovedByUserId = rejectedByUserId;
        ApprovedAt = DateTime.UtcNow;
        RejectionReason = reason;

        // RaiseDomainEvent(new MaterialReturnRequestRejectedEvent(Id, Code, reason));
    }

    public void MarkAsReturned()
    {
        if (Status != MaterialReturnRequestStatus.Approved)
            throw new DomainException("Only approved requests can be marked as returned");

        Status = MaterialReturnRequestStatus.Returned;

        // RaiseDomainEvent(new MaterialReturnRequestReturnedEvent(Id, Code));
    }

    public void MarkAsPartiallyReturned()
    {
        if (Status != MaterialReturnRequestStatus.Approved && Status != MaterialReturnRequestStatus.PartiallyReturned)
            throw new DomainException("Only approved or partially returned requests can be marked as partially returned");

        Status = MaterialReturnRequestStatus.PartiallyReturned;

        // RaiseDomainEvent(new MaterialReturnRequestPartiallyReturnedEvent(Id, Code));
    }

    public void Cancel(string reason)
    {
        if (Status == MaterialReturnRequestStatus.Returned || Status == MaterialReturnRequestStatus.Cancelled)
            throw new DomainException("Cannot cancel a returned or already cancelled request");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Cancellation reason is required");

        Status = MaterialReturnRequestStatus.Cancelled;
        RejectionReason = reason;

        // RaiseDomainEvent(new MaterialReturnRequestCancelledEvent(Id, Code, reason));
    }

    public void Close()
    {
        if (Status != MaterialReturnRequestStatus.Returned && Status != MaterialReturnRequestStatus.PartiallyReturned)
            throw new DomainException("Only returned or partially returned requests can be closed");

        Status = MaterialReturnRequestStatus.Closed;

        // RaiseDomainEvent(new MaterialReturnRequestClosedEvent(Id, Code));
    }

    #endregion

    #region Computed Properties

    public decimal TotalRequestedQuantity => _lines.Sum(l => l.RequestedQuantity);

    public decimal TotalApprovedQuantity => _lines.Sum(l => l.ApprovedQuantity);

    public decimal TotalReturnedQuantity => _lines.Sum(l => l.ReturnedQuantity);

    public bool IsFullyReturned => _lines.All(l => l.ReturnedQuantity >= l.ApprovedQuantity);

    public int GoodConditionLineCount => _lines.Count(l => l.Condition == MaterialCondition.Good);

    public int DamagedLineCount => _lines.Count(l => l.Condition == MaterialCondition.Damaged);

    #endregion

    #region Validation Methods

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Material return request code cannot be empty");

        if (code.Length > 50)
            throw new DomainException("Material return request code cannot exceed 50 characters");
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Material return request title cannot be empty");

        if (title.Length > 200)
            throw new DomainException("Material return request title cannot exceed 200 characters");
    }

    private static void ValidateForeignKey(long foreignKeyId, string fieldName)
    {
        if (foreignKeyId <= 0)
            throw new DomainException($"{fieldName} must be greater than zero");
    }

    #endregion
}

#region Enums

public enum MaterialReturnRequestStatus
{
    Draft = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    PartiallyReturned = 4,
    Returned = 5,
    Closed = 6,
    Cancelled = 7
}

public enum MaterialReturnReason
{
    Unused = 0,
    Excess = 1,
    WrongItem = 2,
    Defective = 3,
    ProjectCancelled = 4,
    Other = 5
}

public enum MaterialCondition
{
    Good = 0,
    Damaged = 1,
    Expired = 2,
    Defective = 3
}

#endregion

#region Child Entities

/// <summary>
/// Entity đại diện cho một dòng trong phiếu yêu cầu trả lại vật tư.
/// </summary>
public class MaterialReturnRequestLine
{
    public long Id { get; private set; }
    public int LineNumber { get; private set; }
    public long ItemId { get; private set; }
    public string ItemCode { get; private set; } = null!;
    public string ItemName { get; private set; } = null!;
    public decimal RequestedQuantity { get; private set; }
    public decimal ApprovedQuantity { get; private set; }
    public decimal ReturnedQuantity { get; private set; }
    public string? Unit { get; private set; }
    public string? LotNumber { get; private set; }
    public string? SerialNumber { get; private set; }
    public MaterialCondition Condition { get; private set; }
    public string? Remarks { get; private set; }

    private MaterialReturnRequestLine() { }

    public MaterialReturnRequestLine(
        int lineNumber,
        long itemId,
        string itemCode,
        string itemName,
        decimal requestedQuantity,
        string? unit = null,
        string? lotNumber = null,
        string? serialNumber = null,
        MaterialCondition condition = MaterialCondition.Good,
        string? remarks = null)
    {
        if (itemId <= 0)
            throw new DomainException("ItemId must be greater than zero");

        if (string.IsNullOrWhiteSpace(itemCode))
            throw new DomainException("Item code cannot be empty");

        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainException("Item name cannot be empty");

        if (requestedQuantity <= 0)
            throw new DomainException("Requested quantity must be greater than zero");

        LineNumber = lineNumber;
        ItemId = itemId;
        ItemCode = itemCode;
        ItemName = itemName;
        RequestedQuantity = requestedQuantity;
        ApprovedQuantity = 0;
        ReturnedQuantity = 0;
        Unit = unit;
        LotNumber = lotNumber;
        SerialNumber = serialNumber;
        Condition = condition;
        Remarks = remarks;
    }

    public void UpdateRequestedQuantity(decimal newQuantity)
    {
        if (newQuantity <= 0)
            throw new DomainException("Requested quantity must be greater than zero");

        RequestedQuantity = newQuantity;
    }

    public void UpdateCondition(MaterialCondition newCondition)
    {
        Condition = newCondition;
    }

    public void SetApprovedQuantity(decimal quantity)
    {
        if (quantity < 0)
            throw new DomainException("Approved quantity cannot be negative");

        if (quantity > RequestedQuantity)
            throw new DomainException("Approved quantity cannot exceed requested quantity");

        ApprovedQuantity = quantity;
    }

    public void AddReturnedQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Returned quantity must be greater than zero");

        if (ReturnedQuantity + quantity > ApprovedQuantity)
            throw new DomainException("Total returned quantity cannot exceed approved quantity");

        ReturnedQuantity += quantity;
    }

    internal void SetLineNumber(int lineNumber)
    {
        LineNumber = lineNumber;
    }
}

#endregion
