using Emm.Domain.Abstractions;
using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Template định nghĩa cấu trúc checkpoint - Aggregate Root
/// Có thể được áp dụng cho nhiều AssetCategory khác nhau
/// </summary>
public class CheckpointTemplate : AggregateRoot
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int Version { get; private set; } // Versioning cho template
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Collections - danh sách các field động
    private readonly List<CheckpointTemplateField> _fields;
    public IReadOnlyCollection<CheckpointTemplateField> Fields => _fields;

    // Collections - danh sách AssetCategory được áp dụng template này
    private readonly List<CheckpointTemplateAssetCategory> _assetCategories;
    public IReadOnlyCollection<CheckpointTemplateAssetCategory> AssetCategories => _assetCategories;

    public CheckpointTemplate(
        string code,
        string name,
        string description)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Template code is required");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Template name is required");

        _fields = [];
        _assetCategories = [];

        Code = code;
        Name = name;
        Description = description;
        IsActive = true;
        Version = 1;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Template name is required");

        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddField(
        string code,
        string name,
        string description,
        FieldValueType fieldType,
        bool isRequired = false,
        int order = 0,
        string? defaultValue = null,
        string? validationRules = null,
        long? masterDataTypeId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Field code is required");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Field name is required");

        // Kiểm tra trùng code
        if (_fields.Any(f => f.Code == code))
            throw new DomainException($"Field with code '{code}' already exists");

        var field = new CheckpointTemplateField(
            Id,
            code,
            name,
            description,
            fieldType,
            isRequired,
            order,
            defaultValue,
            validationRules,
            masterDataTypeId);

        _fields.Add(field);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveField(long fieldId)
    {
        var field = _fields.FirstOrDefault(f => f.Id == fieldId);
        if (field == null)
            throw new DomainException($"Field with ID {fieldId} not found");

        _fields.Remove(field);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFieldOrder(long fieldId, int newOrder)
    {
        var field = _fields.FirstOrDefault(f => f.Id == fieldId);
        if (field == null)
            throw new DomainException($"Field with ID {fieldId} not found");

        field.UpdateOrder(newOrder);
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementVersion()
    {
        Version++;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Áp dụng template cho một AssetCategory
    /// </summary>
    public void ApplyToCategory(long assetCategoryId)
    {
        if (assetCategoryId <= 0)
            throw new DomainException("Invalid asset category ID");

        // Kiểm tra đã tồn tại chưa
        var existing = _assetCategories.FirstOrDefault(ac => ac.AssetCategoryId == assetCategoryId);
        if (existing != null)
        {
            if (existing.IsActive)
                throw new DomainException($"Template already applied to category {assetCategoryId}");

            // Reactivate nếu đã bị remove
            existing.Reactivate();
        }
        else
        {
            var link = new CheckpointTemplateAssetCategory(Id, assetCategoryId);
            _assetCategories.Add(link);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gỡ bỏ template khỏi AssetCategory
    /// </summary>
    public void RemoveFromCategory(long assetCategoryId)
    {
        var link = _assetCategories.FirstOrDefault(ac =>
            ac.AssetCategoryId == assetCategoryId && ac.IsActive);

        if (link == null)
            throw new DomainException($"Template not applied to category {assetCategoryId}");

        link.Deactivate();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Kiểm tra template có được áp dụng cho category không
    /// </summary>
    public bool IsAppliedToCategory(long assetCategoryId)
    {
        return _assetCategories.Any(ac =>
            ac.AssetCategoryId == assetCategoryId && ac.IsActive);
    }

    /// <summary>
    /// Lấy tất cả categories đang áp dụng template này
    /// </summary>
    public IEnumerable<long> GetAppliedCategoryIds()
    {
        return _assetCategories
            .Where(ac => ac.IsActive)
            .Select(ac => ac.AssetCategoryId);
    }

    /// <summary>
    /// Lấy tất cả các field bắt buộc
    /// </summary>
    public IEnumerable<CheckpointTemplateField> GetRequiredFields()
    {
        return _fields.Where(f => f.IsRequired && f.IsActive).OrderBy(f => f.Order);
    }

    /// <summary>
    /// Lấy tất cả các field active theo thứ tự
    /// </summary>
    public IEnumerable<CheckpointTemplateField> GetActiveFields()
    {
        return _fields.Where(f => f.IsActive).OrderBy(f => f.Order);
    }

    private CheckpointTemplate()
    {
        _fields = [];
        _assetCategories = [];
    } // EF Core constructor
}

