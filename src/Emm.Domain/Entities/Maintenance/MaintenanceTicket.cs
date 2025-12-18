using Emm.Domain.Abstractions;
using Emm.Domain.Exceptions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.Maintenance;

/// <summary>
/// Aggregate Root đại diện cho Phiếu bảo trì thiết bị.
/// Quản lý toàn bộ lifecycle của một phiếu bảo trì từ khi tạo đến khi hoàn thành.
/// </summary>
public class MaintenanceTicket : AggregateRoot, IAuditableEntity
{
    private const int MaxTasksPerTicket = 50;
    private const int MaxPartsPerTicket = 100;

    public string Code { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid AssetId { get; private set; }
    public Guid? IncidentReportId { get; private set; }
    public MaintenanceType MaintenanceType { get; private set; }
    public MaintenancePriority Priority { get; private set; }
    public MaintenanceTicketStatus Status { get; private set; }

    public DateTime ScheduledStartDate { get; private set; }
    public DateTime? ScheduledEndDate { get; private set; }
    public DateTime? ActualStartDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }

    public Guid? AssignedToUserId { get; private set; }
    public Guid? AssignedToTeamId { get; private set; }

    public decimal EstimatedCost { get; private set; }
    public decimal ActualCost { get; private set; }
    public decimal EstimatedDurationHours { get; private set; }
    public decimal ActualDurationHours { get; private set; }

    public string? CompletionNotes { get; private set; }
    public string? CancellationReason { get; private set; }

    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    private readonly List<MaintenanceTask> _tasks;
    public IReadOnlyCollection<MaintenanceTask> Tasks => _tasks;

    private readonly List<MaintenancePart> _parts;
    public IReadOnlyCollection<MaintenancePart> Parts => _parts;

    private MaintenanceTicket()
    {
        _tasks = [];
        _parts = [];
    }

    public MaintenanceTicket(
        string code,
        string title,
        Guid assetId,
        MaintenanceType maintenanceType,
        MaintenancePriority priority,
        DateTime scheduledStartDate,
        DateTime? scheduledEndDate = null,
        Guid? incidentReportId = null,
        string? description = null,
        decimal estimatedCost = 0,
        decimal estimatedDurationHours = 0)
    {
        ValidateCode(code);
        ValidateTitle(title);
        DomainGuard.AgainstInvalidForeignKey(assetId, nameof(AssetId));
        ValidateScheduledDates(scheduledStartDate, scheduledEndDate);

        _tasks = [];
        _parts = [];

        Code = code;
        Title = title;
        Description = description;
        AssetId = assetId;
        IncidentReportId = incidentReportId;
        MaintenanceType = maintenanceType;
        Priority = priority;
        Status = MaintenanceTicketStatus.Draft;
        ScheduledStartDate = scheduledStartDate;
        ScheduledEndDate = scheduledEndDate;
        EstimatedCost = estimatedCost;
        EstimatedDurationHours = estimatedDurationHours;

        // RaiseDomainEvent(new MaintenanceTicketCreatedEvent(code, assetId, maintenanceType));
    }

    #region Status Management

    public void Submit()
    {
        if (Status != MaintenanceTicketStatus.Draft)
            throw new DomainException("Only draft tickets can be submitted");

        Status = MaintenanceTicketStatus.Pending;

        // RaiseDomainEvent(new MaintenanceTicketSubmittedEvent(Id, Code));
    }

    public void Approve()
    {
        if (Status != MaintenanceTicketStatus.Pending)
            throw new DomainException("Only pending tickets can be approved");

        Status = MaintenanceTicketStatus.Approved;

        // RaiseDomainEvent(new MaintenanceTicketApprovedEvent(Id, Code));
    }

    public void Reject(string reason)
    {
        if (Status != MaintenanceTicketStatus.Pending)
            throw new DomainException("Only pending tickets can be rejected");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Rejection reason is required");

        Status = MaintenanceTicketStatus.Rejected;
        CancellationReason = reason;

        // RaiseDomainEvent(new MaintenanceTicketRejectedEvent(Id, Code, reason));
    }

    public void StartWork()
    {
        if (Status != MaintenanceTicketStatus.Approved && Status != MaintenanceTicketStatus.Scheduled)
            throw new DomainException("Only approved or scheduled tickets can be started");

        if (!AssignedToUserId.HasValue && !AssignedToTeamId.HasValue)
            throw new DomainException("Ticket must be assigned before starting work");

        Status = MaintenanceTicketStatus.InProgress;
        ActualStartDate = DateTime.UtcNow;

        // RaiseDomainEvent(new MaintenanceTicketStartedEvent(Id, Code));
    }

    public void PauseWork(string reason)
    {
        if (Status != MaintenanceTicketStatus.InProgress)
            throw new DomainException("Only in-progress tickets can be paused");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Pause reason is required");

        Status = MaintenanceTicketStatus.OnHold;

        // RaiseDomainEvent(new MaintenanceTicketPausedEvent(Id, Code, reason));
    }

    public void ResumeWork()
    {
        if (Status != MaintenanceTicketStatus.OnHold)
            throw new DomainException("Only on-hold tickets can be resumed");

        Status = MaintenanceTicketStatus.InProgress;

        // RaiseDomainEvent(new MaintenanceTicketResumedEvent(Id, Code));
    }

    public void Complete(string completionNotes, decimal actualDurationHours)
    {
        if (Status != MaintenanceTicketStatus.InProgress)
            throw new DomainException("Only in-progress tickets can be completed");

        if (actualDurationHours < 0)
            throw new DomainException("Actual duration hours cannot be negative");

        Status = MaintenanceTicketStatus.Completed;
        ActualEndDate = DateTime.UtcNow;
        CompletionNotes = completionNotes;
        ActualDurationHours = actualDurationHours;

        // Calculate actual cost from parts
        ActualCost = _parts.Sum(p => p.TotalCost);

        // RaiseDomainEvent(new MaintenanceTicketCompletedEvent(Id, Code, ActualCost, actualDurationHours));
    }

    public void Cancel(string reason)
    {
        if (Status == MaintenanceTicketStatus.Completed || Status == MaintenanceTicketStatus.Cancelled)
            throw new DomainException("Cannot cancel a completed or already cancelled ticket");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Cancellation reason is required");

        Status = MaintenanceTicketStatus.Cancelled;
        CancellationReason = reason;

        // RaiseDomainEvent(new MaintenanceTicketCancelledEvent(Id, Code, reason));
    }

    #endregion

    #region Assignment

    public void AssignToUser(Guid userId)
    {
        DomainGuard.AgainstInvalidForeignKey(userId, nameof(userId));

        if (Status == MaintenanceTicketStatus.Completed || Status == MaintenanceTicketStatus.Cancelled)
            throw new DomainException("Cannot assign a completed or cancelled ticket");

        AssignedToUserId = userId;

        if (Status == MaintenanceTicketStatus.Approved)
            Status = MaintenanceTicketStatus.Scheduled;

        // RaiseDomainEvent(new MaintenanceTicketAssignedEvent(Id, Code, userId, null));
    }

    public void AssignToTeam(Guid teamId)
    {
        DomainGuard.AgainstInvalidForeignKey(teamId, nameof(teamId));

        if (Status == MaintenanceTicketStatus.Completed || Status == MaintenanceTicketStatus.Cancelled)
            throw new DomainException("Cannot assign a completed or cancelled ticket");

        AssignedToTeamId = teamId;

        if (Status == MaintenanceTicketStatus.Approved)
            Status = MaintenanceTicketStatus.Scheduled;

        // RaiseDomainEvent(new MaintenanceTicketAssignedEvent(Id, Code, null, teamId));
    }

    public void Unassign()
    {
        if (Status == MaintenanceTicketStatus.InProgress)
            throw new DomainException("Cannot unassign an in-progress ticket");

        AssignedToUserId = null;
        AssignedToTeamId = null;

        if (Status == MaintenanceTicketStatus.Scheduled)
            Status = MaintenanceTicketStatus.Approved;

        // RaiseDomainEvent(new MaintenanceTicketUnassignedEvent(Id, Code));
    }

    #endregion

    #region Update Info

    public void UpdateInfo(
        string title,
        string? description,
        MaintenancePriority priority,
        DateTime scheduledStartDate,
        DateTime? scheduledEndDate,
        decimal estimatedCost,
        decimal estimatedDurationHours)
    {
        if (Status == MaintenanceTicketStatus.Completed || Status == MaintenanceTicketStatus.Cancelled)
            throw new DomainException("Cannot update a completed or cancelled ticket");

        ValidateTitle(title);
        ValidateScheduledDates(scheduledStartDate, scheduledEndDate);

        Title = title;
        Description = description;
        Priority = priority;
        ScheduledStartDate = scheduledStartDate;
        ScheduledEndDate = scheduledEndDate;
        EstimatedCost = estimatedCost;
        EstimatedDurationHours = estimatedDurationHours;

        // RaiseDomainEvent(new MaintenanceTicketUpdatedEvent(Id, Code));
    }

    public void UpdatePriority(MaintenancePriority newPriority)
    {
        if (Status == MaintenanceTicketStatus.Completed || Status == MaintenanceTicketStatus.Cancelled)
            throw new DomainException("Cannot update priority of a completed or cancelled ticket");

        if (Priority == newPriority)
            return;

        var oldPriority = Priority;
        Priority = newPriority;

        // RaiseDomainEvent(new MaintenanceTicketPriorityChangedEvent(Id, Code, oldPriority, newPriority));
    }

    public void Reschedule(DateTime newScheduledStartDate, DateTime? newScheduledEndDate)
    {
        if (Status == MaintenanceTicketStatus.Completed || Status == MaintenanceTicketStatus.Cancelled)
            throw new DomainException("Cannot reschedule a completed or cancelled ticket");

        ValidateScheduledDates(newScheduledStartDate, newScheduledEndDate);

        ScheduledStartDate = newScheduledStartDate;
        ScheduledEndDate = newScheduledEndDate;

        // RaiseDomainEvent(new MaintenanceTicketRescheduledEvent(Id, Code, newScheduledStartDate, newScheduledEndDate));
    }

    #endregion

    #region Tasks Management

    public void AddTask(
        string taskName,
        string? taskDescription = null,
        decimal estimatedDurationMinutes = 0,
        int sequenceOrder = 0)
    {
        if (Status == MaintenanceTicketStatus.Completed || Status == MaintenanceTicketStatus.Cancelled)
            throw new DomainException("Cannot add tasks to a completed or cancelled ticket");

        if (_tasks.Count >= MaxTasksPerTicket)
            throw new DomainException($"Cannot exceed {MaxTasksPerTicket} tasks per ticket");

        var task = new MaintenanceTask(
            taskName,
            taskDescription,
            estimatedDurationMinutes,
            sequenceOrder == 0 ? _tasks.Count + 1 : sequenceOrder);

        _tasks.Add(task);

        // RaiseDomainEvent(new MaintenanceTaskAddedEvent(Id, taskName));
    }

    public void CompleteTask(Guid taskId, string? notes = null)
    {
        if (Status != MaintenanceTicketStatus.InProgress)
            throw new DomainException("Tasks can only be completed when ticket is in progress");

        var task = _tasks.FirstOrDefault(t => t.Id == taskId)
            ?? throw new DomainException($"Task {taskId} not found in this ticket");

        task.Complete(notes);

        // RaiseDomainEvent(new MaintenanceTaskCompletedEvent(Id, taskId));
    }

    public void RemoveTask(Guid taskId)
    {
        if (Status == MaintenanceTicketStatus.Completed || Status == MaintenanceTicketStatus.Cancelled)
            throw new DomainException("Cannot remove tasks from a completed or cancelled ticket");

        var task = _tasks.FirstOrDefault(t => t.Id == taskId)
            ?? throw new DomainException($"Task {taskId} not found in this ticket");

        if (task.IsCompleted)
            throw new DomainException("Cannot remove a completed task");

        _tasks.Remove(task);

        // RaiseDomainEvent(new MaintenanceTaskRemovedEvent(Id, taskId));
    }

    #endregion

    #region Parts Management

    public void AddPart(
        Guid itemId,
        string itemCode,
        string itemName,
        decimal quantity,
        decimal unitCost,
        string? unit = null)
    {
        if (Status == MaintenanceTicketStatus.Completed || Status == MaintenanceTicketStatus.Cancelled)
            throw new DomainException("Cannot add parts to a completed or cancelled ticket");

        if (_parts.Count >= MaxPartsPerTicket)
            throw new DomainException($"Cannot exceed {MaxPartsPerTicket} parts per ticket");

        if (_parts.Any(p => p.ItemId == itemId))
            throw new DomainException($"Item {itemCode} already exists in this ticket. Use UpdatePartQuantity instead.");

        var part = new MaintenancePart(itemId, itemCode, itemName, quantity, unitCost, unit);
        _parts.Add(part);

        // RaiseDomainEvent(new MaintenancePartAddedEvent(Id, itemId, quantity));
    }

    public void UpdatePartQuantity(Guid itemId, decimal newQuantity)
    {
        if (Status == MaintenanceTicketStatus.Completed || Status == MaintenanceTicketStatus.Cancelled)
            throw new DomainException("Cannot update parts in a completed or cancelled ticket");

        var part = _parts.FirstOrDefault(p => p.ItemId == itemId)
            ?? throw new DomainException($"Part with ItemId {itemId} not found in this ticket");

        part.UpdateQuantity(newQuantity);

        // RaiseDomainEvent(new MaintenancePartQuantityUpdatedEvent(Id, itemId, newQuantity));
    }

    public void RemovePart(Guid itemId)
    {
        if (Status == MaintenanceTicketStatus.Completed || Status == MaintenanceTicketStatus.Cancelled)
            throw new DomainException("Cannot remove parts from a completed or cancelled ticket");

        var part = _parts.FirstOrDefault(p => p.ItemId == itemId)
            ?? throw new DomainException($"Part with ItemId {itemId} not found in this ticket");

        _parts.Remove(part);

        // RaiseDomainEvent(new MaintenancePartRemovedEvent(Id, itemId));
    }

    #endregion

    #region Computed Properties

    public bool IsOverdue => Status != MaintenanceTicketStatus.Completed
        && Status != MaintenanceTicketStatus.Cancelled
        && ScheduledEndDate.HasValue
        && DateTime.UtcNow > ScheduledEndDate.Value;

    public decimal TotalPartsCount => _parts.Sum(p => p.Quantity);

    public decimal TotalPartsCost => _parts.Sum(p => p.TotalCost);

    public int CompletedTasksCount => _tasks.Count(t => t.IsCompleted);

    public int TotalTasksCount => _tasks.Count;

    public decimal TaskCompletionPercentage => TotalTasksCount == 0
        ? 0
        : Math.Round((decimal)CompletedTasksCount / TotalTasksCount * 100, 2);

    #endregion

    #region Validation Methods

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Maintenance ticket code cannot be empty");

        if (code.Length > 50)
            throw new DomainException("Maintenance ticket code cannot exceed 50 characters");
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Maintenance ticket title cannot be empty");

        if (title.Length > 200)
            throw new DomainException("Maintenance ticket title cannot exceed 200 characters");
    }

    private static void ValidateScheduledDates(DateTime startDate, DateTime? endDate)
    {
        if (endDate.HasValue && endDate.Value < startDate)
            throw new DomainException("Scheduled end date cannot be before start date");
    }

    #endregion
}

#region Enums

public enum MaintenanceTicketStatus
{
    Draft = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Scheduled = 4,
    InProgress = 5,
    OnHold = 6,
    Completed = 7,
    Cancelled = 8
}

public enum MaintenanceType
{
    Preventive = 0,
    Corrective = 1,
    Predictive = 2,
    Emergency = 3,
    Inspection = 4
}

public enum MaintenancePriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

#endregion

#region Value Objects / Child Entities

/// <summary>
/// Entity đại diện cho một công việc cần thực hiện trong phiếu bảo trì.
/// </summary>
public class MaintenanceTask
{
    public Guid Id { get; private set; }
    public string TaskName { get; private set; } = null!;
    public string? TaskDescription { get; private set; }
    public decimal EstimatedDurationMinutes { get; private set; }
    public decimal ActualDurationMinutes { get; private set; }
    public int SequenceOrder { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? CompletionNotes { get; private set; }

    private MaintenanceTask() { }

    public MaintenanceTask(
        string taskName,
        string? taskDescription,
        decimal estimatedDurationMinutes,
        int sequenceOrder)
    {
        if (string.IsNullOrWhiteSpace(taskName))
            throw new DomainException("Task name cannot be empty");

        if (taskName.Length > 200)
            throw new DomainException("Task name cannot exceed 200 characters");

        if (estimatedDurationMinutes < 0)
            throw new DomainException("Estimated duration cannot be negative");

        TaskName = taskName;
        TaskDescription = taskDescription;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        SequenceOrder = sequenceOrder;
        IsCompleted = false;
    }

    public void Complete(string? notes = null, decimal actualDurationMinutes = 0)
    {
        if (IsCompleted)
            throw new DomainException("Task is already completed");

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        CompletionNotes = notes;
        ActualDurationMinutes = actualDurationMinutes;
    }

    public void UpdateEstimatedDuration(decimal newDuration)
    {
        if (IsCompleted)
            throw new DomainException("Cannot update estimated duration of a completed task");

        if (newDuration < 0)
            throw new DomainException("Duration cannot be negative");

        EstimatedDurationMinutes = newDuration;
    }
}

/// <summary>
/// Entity đại diện cho vật tư/phụ tùng sử dụng trong phiếu bảo trì.
/// </summary>
public class MaintenancePart
{
    public Guid Id { get; private set; }
    public Guid ItemId { get; private set; }
    public string ItemCode { get; private set; } = null!;
    public string ItemName { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public string? Unit { get; private set; }
    public decimal TotalCost => Quantity * UnitCost;

    private MaintenancePart() { }

    public MaintenancePart(
        Guid itemId,
        string itemCode,
        string itemName,
        decimal quantity,
        decimal unitCost,
        string? unit = null)
    {
        DomainGuard.AgainstInvalidForeignKey(itemId, nameof(ItemId));

        if (string.IsNullOrWhiteSpace(itemCode))
            throw new DomainException("Item code cannot be empty");

        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainException("Item name cannot be empty");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        if (unitCost < 0)
            throw new DomainException("Unit cost cannot be negative");

        ItemId = itemId;
        ItemCode = itemCode;
        ItemName = itemName;
        Quantity = quantity;
        UnitCost = unitCost;
        Unit = unit;
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
}

#endregion
