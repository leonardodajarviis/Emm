namespace Emm.Domain.Entities.AssetCatalog;

/// <summary>
/// Vật tư phụ tùng cần thiết cho kế hoạch bảo trì
/// </summary>
public class MaintenancePlanRequiredItem
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid MaintenancePlanDefinitionId { get; private set; }

    /// <summary>
    /// ID của vật tư/phụ tùng
    /// </summary>
    public Guid ItemId { get; private set; }

    public Guid UnitOfMeasureId { get; private set; }

    public Guid ItemGroupId { get; private set; }

    /// <summary>
    /// Số lượng cần thiết
    /// </summary>
    public decimal Quantity { get; private set; }

    /// <summary>
    /// Có bắt buộc không (true = bắt buộc, false = tùy chọn)
    /// </summary>
    public bool IsRequired { get; private set; }

    /// <summary>
    /// Ghi chú về vật tư này
    /// </summary>
    public string? Note { get; private set; }

    public MaintenancePlanRequiredItem(Guid itemGroupId, Guid itemId, Guid unitOfMeasureId, decimal quantity, bool isRequired, string? note = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        ItemGroupId = itemGroupId;
        ItemId = itemId;
        UnitOfMeasureId = unitOfMeasureId;
        Quantity = quantity;
        IsRequired = isRequired;
        Note = note;
    }

    public void Update(decimal quantity, bool isRequired, string? note)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        Quantity = quantity;
        IsRequired = isRequired;
        Note = note;
    }

    private MaintenancePlanRequiredItem()
    {
    } // EF Core constructor
}
