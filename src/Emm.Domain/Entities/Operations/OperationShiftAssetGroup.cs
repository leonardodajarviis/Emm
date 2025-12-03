using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Metadata entity để nhóm các tài sản trong ca vận hành
/// Ví dụ: "Nhóm máy chính", "Nhóm máy phụ trợ", "Nhóm máy dự phòng"
/// </summary>
public class OperationShiftAssetGroup
{
    public long Id { get; private set; }
    public Guid LinkedId { get; private set; }
    public long OperationShiftId { get; private set; }
    public string GroupName { get; private set; } = null!;
    public string? Description { get; private set; }
    public GroupRole Role { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public OperationShiftAssetGroup(
        long operationShiftId,
        Guid linkedId,
        string groupName,
        GroupRole role = GroupRole.Secondary,
        int displayOrder = 0,
        string? description = null)
    {
        if (operationShiftId <= 0)
            throw new DomainException("OperationShiftId must be greater than zero");

        if (linkedId == Guid.Empty)
            throw new DomainException("LinkedId cannot be empty");

        if (string.IsNullOrWhiteSpace(groupName))
            throw new DomainException("Group name cannot be empty");

        if (groupName.Length > 200)
            throw new DomainException("Group name cannot exceed 200 characters");

        OperationShiftId = operationShiftId;
        LinkedId = linkedId;
        GroupName = groupName;
        Role = role;
        DisplayOrder = displayOrder;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string groupName, GroupRole role, int displayOrder, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            throw new DomainException("Group name cannot be empty");

        if (groupName.Length > 200)
            throw new DomainException("Group name cannot exceed 200 characters");

        GroupName = groupName;
        Role = role;
        DisplayOrder = displayOrder;
        Description = description;
    }

    private OperationShiftAssetGroup() { } // EF Core constructor
}

/// <summary>
/// Vai trò của nhóm tài sản trong ca vận hành
/// </summary>
public enum GroupRole
{
    Primary = 1,      // Nhóm máy chính
    Secondary = 2,    // Nhóm máy phụ trợ
    Backup = 3,       // Nhóm máy dự phòng
    Support = 4       // Nhóm máy hỗ trợ
}
