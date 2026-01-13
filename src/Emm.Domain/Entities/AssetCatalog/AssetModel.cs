using Emm.Domain.Abstractions;
using Emm.Domain.Exceptions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.AssetCatalog;

public class AssetModel : AggregateRoot, IAuditableEntity
{
    private const int MaxImagesPerModel = 50;
    private const int MaxParametersPerModel = 200;

    public bool IsCodeGenerated { get; private set; }
    public NaturalKey Code { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? Notes { get; private set; }
    public Guid? ParentId { get; private set; }
    public Guid AssetCategoryId { get; private set; }
    public Guid AssetTypeId { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? ThumbnailFileId { get; private set; }
    public string? ThumbnailUrl { get; private set; }

    private readonly List<AssetModelParameter> _parameters;
    public IReadOnlyCollection<AssetModelParameter> Parameters => _parameters;


    private readonly List<AssetModelImage> _images;
    public IReadOnlyCollection<AssetModelImage> Images => _images;


    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    private AssetModel()
    {
        _parameters = [];
        _images = [];
    } // EF Core constructor

    public AssetModel(
        bool isCodeGenerated,
        NaturalKey code,
        string name,
        Guid assetCategoryId,
        Guid assetTypeId,
        string? description = null,
        string? notes = null,
        Guid? parentId = null,
        bool isActive = true)
    {
        _parameters = [];
        _images = [];

        Code = code;
        Name = name;
        Description = description;
        Notes = notes;
        ParentId = parentId;
        AssetCategoryId = assetCategoryId;
        AssetTypeId = assetTypeId;
        IsActive = isActive;
        IsCodeGenerated = isCodeGenerated;

        // RaiseDomainEvent(new AssetModelCreatedEvent(code, name));
    }

    public void SetThumbnailOnCreate(Guid fileId, string fileUrl)
    {
        ThumbnailFileId = fileId;
        ThumbnailUrl = fileUrl;
    }

    public void UpdateThumbnail(Guid fileId, string fileUrl)
    {
        ThumbnailFileId = fileId;
        ThumbnailUrl = fileUrl;

        // RaiseDomainEvent(new AssetModelThumbnailUpdatedEvent(Id, fileId));
    }

    public void RemoveThumbnail()
    {
        if (ThumbnailFileId.HasValue)
        {
            var removedFileId = ThumbnailFileId.Value;
            ThumbnailFileId = null;
            ThumbnailUrl = null;

            // RaiseDomainEvent(new AssetModelThumbnailRemovedEvent(Id, removedFileId));
        }
    }

    public void Update(
        string name,
        string? description,
        string? notes,
        Guid? parentId,
        Guid assetCategoryId,
        Guid assetTypeId,
        bool isActive)
    {
        ValidateParentId(parentId);

        Name = name;
        Description = description;
        Notes = notes;
        ParentId = parentId;
        AssetCategoryId = assetCategoryId;
        AssetTypeId = assetTypeId;
        IsActive = isActive;

        // RaiseDomainEvent(new AssetModelUpdatedEvent(Id, Name));
    }

    public void AddParameter(Guid parameterId)
    {
        DomainGuard.AgainstBusinessRule(
            _parameters.Count >= MaxParametersPerModel,
            "MaxParametersLimit",
            $"Cannot exceed {MaxParametersPerModel} parameters per asset model");

        var parameter = new AssetModelParameter(parameterId);
        _parameters.Add(parameter);

        // RaiseDomainEvent(new AssetModelParameterAddedEvent(Id, parameterId));
    }

    public void AddParameters(params Guid[] parameterIds)
    {
        if (parameterIds == null || parameterIds.Length == 0)
            return;

        var distinctIds = parameterIds.Distinct().ToArray();

        // Check limit
        var newParametersCount = distinctIds.Count(id => !_parameters.Any(p => p.ParameterId == id));
        DomainGuard.AgainstBusinessRule(
            _parameters.Count + newParametersCount > MaxParametersPerModel,
            "MaxParametersLimit",
            $"Cannot exceed {MaxParametersPerModel} parameters per asset model");

        // Add parameters by reusing AddParameter logic
        foreach (var id in distinctIds)
        {
            if (!_parameters.Any(p => p.ParameterId == id))
            {
                AddParameter(id);
            }
        }
    }

    /// <summary>
    /// Internal method for finding a parameter. Should only be called by MaintenancePlanManagementService.
    /// </summary>
    public AssetModelParameter? FindParameter(Guid parameterId)
    {
        return _parameters.FirstOrDefault(p => p.ParameterId == parameterId);
    }

    public void AddImage(Guid fileId, string filePath)
    {
        DomainGuard.AgainstBusinessRule(
            _images.Count >= MaxImagesPerModel,
            "MaxImagesLimit",
            $"Cannot exceed {MaxImagesPerModel} images per asset model");

        var image = new AssetModelImage(fileId, filePath);
        _images.Add(image);

        // RaiseDomainEvent(new AssetModelImageAddedEvent(Id, fileId));
    }

    public void RemoveImage(Guid fileId)
    {
        var image = _images.FirstOrDefault(img => img.FileId == fileId);
        _images.Remove(image!);

        // RaiseDomainEvent(new AssetModelImageRemovedEvent(Id, fileId));
    }

    private void ValidateParentId(Guid? parentId)
    {
        if (!parentId.HasValue)
            return;
    }
}

public class AssetModelImage
{
    public Guid AssetModelId { get; private set; }
    public Guid FileId { get; private set; }
    public string FilePath { get; private set; } = null!;

    public AssetModelImage(Guid fileId, string filePath)
    {
        FileId = fileId;
        FilePath = filePath;
    }

    private AssetModelImage()
    {
    } // EF Core constructor
}
