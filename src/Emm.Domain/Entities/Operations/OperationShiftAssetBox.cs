using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Metadata entity để nhóm các tài sản trong ca vận hành
/// Ví dụ: "Nhóm máy chính", "Nhóm máy phụ trợ", "Nhóm máy dự phòng"
/// </summary>
public class OperationShiftAssetBox
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid OperationShiftId { get; private set; }
    public string BoxName { get; private set; } = null!;
    public string? Description { get; private set; }
    public BoxRole Role { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public OperationShiftAssetBox(
        Guid operationShiftId,
        string boxName,
        BoxRole role = BoxRole.Secondary,
        int displayOrder = 0,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(boxName))
            throw new DomainException("Box name cannot be empty");

        if (boxName.Length > 200)
            throw new DomainException("Box name cannot exceed 200 characters");

        OperationShiftId = operationShiftId;
        BoxName = boxName;
        Role = role;
        DisplayOrder = displayOrder;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string boxName, BoxRole role, int displayOrder, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(boxName))
            throw new DomainException("Group name cannot be empty");

        if (boxName.Length > 200)
            throw new DomainException("Group name cannot exceed 200 characters");

        BoxName = boxName;
        Role = role;
        DisplayOrder = displayOrder;
        Description = description;
    }

    private OperationShiftAssetBox() { } // EF Core constructor
}

/// <summary>
/// Vai trò của nhóm tài sản trong ca vận hành
/// </summary>
public enum BoxRole
{
    Primary = 1,      // Nhóm máy chính
    Secondary = 2,    // Nhóm máy phụ trợ
    Backup = 3,       // Nhóm máy dự phòng
    Support = 4       // Nhóm máy hỗ trợ
}
