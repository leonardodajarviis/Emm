using Emm.Domain.Abstractions;
using Emm.Domain.Exceptions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.Inventory;

/// <summary>
/// Aggregate Root đại diện cho Phiếu yêu cầu vật tư.
/// Dùng để yêu cầu xuất vật tư từ kho cho các mục đích như bảo trì, sản xuất, v.v.
/// </summary>
public class MaterialRequest : AggregateRoot, IAuditableEntity
{
    private const int MaxLinesPerRequest = 100;

    public string Code { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid RequestedByUserId { get; private set; }
    public Guid OrganizationUnitId { get; private set; }
    public Guid? WarehouseId { get; private set; }
    public Guid? MaintenanceTicketId { get; private set; }
    public MaterialRequestPurpose Purpose { get; private set; }
    public MaterialRequestStatus Status { get; private set; }
    public MaterialRequestPriority Priority { get; private set; }

    public DateTime RequestedDate { get; private set; }
    public DateTime? RequiredByDate { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }
    public string? ApprovalNotes { get; private set; }
    public string? RejectionReason { get; private set; }
    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    private readonly List<MaterialRequestLine> _lines;
    public IReadOnlyCollection<MaterialRequestLine> Lines => _lines;

    private MaterialRequest()
    {
        _lines = [];
    }

    public MaterialRequest(
        string code,
        string title,
        Guid requestedByUserId,
        Guid organizationUnitId,
        MaterialRequestPurpose purpose,
        MaterialRequestPriority priority = MaterialRequestPriority.Normal,
        DateTime? requiredByDate = null,
        Guid? warehouseId = null,
        Guid? maintenanceTicketId = null,
        string? description = null)
    {
        ValidateCode(code);
        ValidateTitle(title);

        _lines = [];

        Code = code;
        Title = title;
        Description = description;
        RequestedByUserId = requestedByUserId;
        OrganizationUnitId = organizationUnitId;
        WarehouseId = warehouseId;
        MaintenanceTicketId = maintenanceTicketId;
        Purpose = purpose;
        Priority = priority;
        Status = MaterialRequestStatus.Draft;
        RequestedDate = DateTime.UtcNow;
        RequiredByDate = requiredByDate;

        // RaiseDomainEvent(new MaterialRequestCreatedEvent(code, organizationUnitId, purpose));
    }

    #region Line Management

    public void AddLine(
        Guid itemId,
        string itemCode,
        string itemName,
        decimal requestedQuantity,
        string? unit = null,
        string? remarks = null)
    {
        if (Status != MaterialRequestStatus.Draft)
            throw new DomainException("Can only add lines to draft requests");

        if (_lines.Count >= MaxLinesPerRequest)
            throw new DomainException($"Cannot exceed {MaxLinesPerRequest} lines per request");

        if (_lines.Any(l => l.ItemId == itemId))
            throw new DomainException($"Item {itemCode} already exists in this request");

        var lineNumber = _lines.Count + 1;
        var line = new MaterialRequestLine(lineNumber, itemId, itemCode, itemName, requestedQuantity, unit, remarks);
        _lines.Add(line);

        // RaiseDomainEvent(new MaterialRequestLineAddedEvent(Id, itemId, requestedQuantity));
    }

    public void UpdateLineQuantity(Guid itemId, decimal newQuantity)
    {
        if (Status != MaterialRequestStatus.Draft)
            throw new DomainException("Can only update lines in draft requests");

        var line = _lines.FirstOrDefault(l => l.ItemId == itemId)
            ?? throw new DomainException($"Item with Id {itemId} not found in this request");

        line.UpdateRequestedQuantity(newQuantity);

        // RaiseDomainEvent(new MaterialRequestLineUpdatedEvent(Id, itemId, newQuantity));
    }

    public void RemoveLine(Guid itemId)
    {
        if (Status != MaterialRequestStatus.Draft)
            throw new DomainException("Can only remove lines from draft requests");

        var line = _lines.FirstOrDefault(l => l.ItemId == itemId)
            ?? throw new DomainException($"Item with Id {itemId} not found in this request");

        _lines.Remove(line);
        ReorderLines();

        // RaiseDomainEvent(new MaterialRequestLineRemovedEvent(Id, itemId));
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
        if (Status != MaterialRequestStatus.Draft)
            throw new DomainException("Only draft requests can be submitted");

        if (!_lines.Any())
            throw new DomainException("Cannot submit a request without any lines");

        Status = MaterialRequestStatus.Pending;

        // RaiseDomainEvent(new MaterialRequestSubmittedEvent(Id, Code));
    }

    public void Approve(Guid approvedByUserId, string? notes = null)
    {
        if (Status != MaterialRequestStatus.Pending)
            throw new DomainException("Only pending requests can be approved");

        Status = MaterialRequestStatus.Approved;
        ApprovedByUserId = approvedByUserId;
        ApprovedAt = DateTime.UtcNow;
        ApprovalNotes = notes;

        // RaiseDomainEvent(new MaterialRequestApprovedEvent(Id, Code, approvedByUserId));
    }

    public void Reject(Guid rejectedByUserId, string reason)
    {
        if (Status != MaterialRequestStatus.Pending)
            throw new DomainException("Only pending requests can be rejected");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Rejection reason is required");

        Status = MaterialRequestStatus.Rejected;
        ApprovedByUserId = rejectedByUserId;
        ApprovedAt = DateTime.UtcNow;
        RejectionReason = reason;

        // RaiseDomainEvent(new MaterialRequestRejectedEvent(Id, Code, reason));
    }

    public void MarkAsIssued()
    {
        if (Status != MaterialRequestStatus.Approved)
            throw new DomainException("Only approved requests can be marked as issued");

        Status = MaterialRequestStatus.Issued;

        // RaiseDomainEvent(new MaterialRequestIssuedEvent(Id, Code));
    }

    public void MarkAsPartiallyIssued()
    {
        if (Status != MaterialRequestStatus.Approved && Status != MaterialRequestStatus.PartiallyIssued)
            throw new DomainException("Only approved or partially issued requests can be marked as partially issued");

        Status = MaterialRequestStatus.PartiallyIssued;

        // RaiseDomainEvent(new MaterialRequestPartiallyIssuedEvent(Id, Code));
    }

    public void Cancel(string reason)
    {
        if (Status == MaterialRequestStatus.Issued || Status == MaterialRequestStatus.Cancelled)
            throw new DomainException("Cannot cancel an issued or already cancelled request");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Cancellation reason is required");

        Status = MaterialRequestStatus.Cancelled;
        RejectionReason = reason;

        // RaiseDomainEvent(new MaterialRequestCancelledEvent(Id, Code, reason));
    }

    public void Close()
    {
        if (Status != MaterialRequestStatus.Issued && Status != MaterialRequestStatus.PartiallyIssued)
            throw new DomainException("Only issued or partially issued requests can be closed");

        Status = MaterialRequestStatus.Closed;

        // RaiseDomainEvent(new MaterialRequestClosedEvent(Id, Code));
    }

    #endregion

    #region Update Info

    public void UpdateInfo(
        string title,
        string? description,
        MaterialRequestPriority priority,
        DateTime? requiredByDate,
        Guid? warehouseId)
    {
        if (Status != MaterialRequestStatus.Draft)
            throw new DomainException("Can only update draft requests");

        ValidateTitle(title);

        Title = title;
        Description = description;
        Priority = priority;
        RequiredByDate = requiredByDate;
        WarehouseId = warehouseId;

        // RaiseDomainEvent(new MaterialRequestUpdatedEvent(Id, Code));
    }

    #endregion

    #region Computed Properties

    public decimal TotalRequestedQuantity => _lines.Sum(l => l.RequestedQuantity);

    public decimal TotalApprovedQuantity => _lines.Sum(l => l.ApprovedQuantity);

    public decimal TotalIssuedQuantity => _lines.Sum(l => l.IssuedQuantity);

    public bool IsFullyIssued => _lines.All(l => l.IssuedQuantity >= l.ApprovedQuantity);

    public bool IsOverdue => RequiredByDate.HasValue
        && Status != MaterialRequestStatus.Issued
        && Status != MaterialRequestStatus.Closed
        && Status != MaterialRequestStatus.Cancelled
        && DateTime.UtcNow > RequiredByDate.Value;

    #endregion

    #region Validation Methods

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Material request code cannot be empty");

        if (code.Length > 50)
            throw new DomainException("Material request code cannot exceed 50 characters");
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Material request title cannot be empty");

        if (title.Length > 200)
            throw new DomainException("Material request title cannot exceed 200 characters");
    }

    #endregion
}

#region Enums

public enum MaterialRequestStatus
{
    Draft = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    PartiallyIssued = 4,
    Issued = 5,
    Closed = 6,
    Cancelled = 7
}

public enum MaterialRequestPurpose
{
    Maintenance = 0,
    Production = 1,
    Project = 2,
    Emergency = 3,
    Other = 4
}

public enum MaterialRequestPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}

#endregion

#region Child Entities

/// <summary>
/// Entity đại diện cho một dòng trong phiếu yêu cầu vật tư.
/// </summary>
public class MaterialRequestLine
{
    public Guid Id { get; private set; }
    public int LineNumber { get; private set; }
    public Guid ItemId { get; private set; }
    public string ItemCode { get; private set; } = null!;
    public string ItemName { get; private set; } = null!;
    public decimal RequestedQuantity { get; private set; }
    public decimal ApprovedQuantity { get; private set; }
    public decimal IssuedQuantity { get; private set; }
    public string? Unit { get; private set; }
    public string? Remarks { get; private set; }

    private MaterialRequestLine() { }

    public MaterialRequestLine(
        int lineNumber,
        Guid itemId,
        string itemCode,
        string itemName,
        decimal requestedQuantity,
        string? unit = null,
        string? remarks = null)
    {
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
        IssuedQuantity = 0;
        Unit = unit;
        Remarks = remarks;
    }

    public void UpdateRequestedQuantity(decimal newQuantity)
    {
        if (newQuantity <= 0)
            throw new DomainException("Requested quantity must be greater than zero");

        RequestedQuantity = newQuantity;
    }

    public void SetApprovedQuantity(decimal quantity)
    {
        if (quantity < 0)
            throw new DomainException("Approved quantity cannot be negative");

        if (quantity > RequestedQuantity)
            throw new DomainException("Approved quantity cannot exceed requested quantity");

        ApprovedQuantity = quantity;
    }

    public void AddIssuedQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Issued quantity must be greater than zero");

        if (IssuedQuantity + quantity > ApprovedQuantity)
            throw new DomainException("Total issued quantity cannot exceed approved quantity");

        IssuedQuantity += quantity;
    }

    internal void SetLineNumber(int lineNumber)
    {
        LineNumber = lineNumber;
    }
}

#endregion
