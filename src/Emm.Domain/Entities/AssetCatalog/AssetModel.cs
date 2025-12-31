using Emm.Domain.Abstractions;
using Emm.Domain.Exceptions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.AssetCatalog;

public class AssetModel : AggregateRoot, IAuditableEntity
{
    private const int MaxMaintenancePlansPerModel = 100;
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

    private readonly List<MaintenancePlanDefinition> _maintenancePlanDefinitions;
    public IReadOnlyCollection<MaintenancePlanDefinition> MaintenancePlanDefinitions => _maintenancePlanDefinitions;

    private readonly List<AssetModelImage> _images;
    public IReadOnlyCollection<AssetModelImage> Images => _images;

    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    private AssetModel()
    {
        _parameters = [];
        _maintenancePlanDefinitions = [];
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

        DomainGuard.AgainstInvalidForeignKey(assetCategoryId, nameof(AssetCategoryId));
        DomainGuard.AgainstInvalidForeignKey(assetTypeId, nameof(AssetTypeId));

        _parameters = [];
        _maintenancePlanDefinitions = [];
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
        DomainGuard.AgainstInvalidForeignKey(assetCategoryId, nameof(AssetCategoryId));
        DomainGuard.AgainstInvalidForeignKey(assetTypeId, nameof(AssetTypeId));

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
        DomainGuard.AgainstInvalidForeignKey(parameterId, nameof(parameterId));
        DomainGuard.AgainstBusinessRule(
            _parameters.Count >= MaxParametersPerModel,
            "MaxParametersLimit",
            $"Cannot exceed {MaxParametersPerModel} parameters per asset model");
        DomainGuard.AgainstDuplicate(
            _parameters.Any(p => p.ParameterId == parameterId),
            nameof(AssetModelParameter),
            nameof(parameterId),
            parameterId);

        var parameter = new AssetModelParameter(parameterId);
        _parameters.Add(parameter);

        // RaiseDomainEvent(new AssetModelParameterAddedEvent(Id, parameterId));
    }

    public void AddParameters(params Guid[] parameterIds)
    {
        if (parameterIds == null || parameterIds.Length == 0)
            return;

        var distinctIds = parameterIds.Distinct().ToArray();

        // Validate all IDs first
        foreach (var id in distinctIds)
        {
            DomainGuard.AgainstInvalidForeignKey(id, "ParameterId");
        }

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

    public void RemoveParameter(Guid parameterId)
    {
        DomainGuard.AgainstInvalidForeignKey(parameterId, nameof(parameterId));

        var parameter = _parameters.FirstOrDefault(x => x.ParameterId == parameterId);
        DomainGuard.AgainstNotFound(parameter, nameof(AssetModelParameter), parameterId);

        _parameters.Remove(parameter!);

        // RaiseDomainEvent(new AssetModelParameterRemovedEvent(Id, parameterId));
    }

    public void RemoveParameters(IReadOnlyCollection<Guid> parameterIds, bool throwIfNotFound = false)
    {
        if (parameterIds == null || parameterIds.Count == 0)
            return;

        foreach (var parameterId in parameterIds)
        {
            DomainGuard.AgainstInvalidForeignKey(parameterId, nameof(parameterId));

            var parameter = _parameters.FirstOrDefault(x => x.ParameterId == parameterId);

            if (parameter == null)
            {
                if (throwIfNotFound)
                    DomainGuard.AgainstNotFound(parameter, nameof(AssetModelParameter), parameterId);
                continue;
            }

            _parameters.Remove(parameter);
        }
    }

    // Internal methods for Domain Service to use
    // These methods should ONLY be called by MaintenancePlanManagementService

    /// <summary>
    /// Internal method for adding a maintenance plan. Should only be called by MaintenancePlanManagementService.
    /// </summary>
    public void AddMaintenancePlanInternal(MaintenancePlanDefinition plan)
    {
        ValidateMaintenancePlanLimit();
        DomainGuard.AgainstNull(plan, nameof(plan));
        _maintenancePlanDefinitions.Add(plan);
    }

    /// <summary>
    /// Internal method for removing a maintenance plan. Should only be called by MaintenancePlanManagementService.
    /// </summary>
    public void RemoveMaintenancePlanInternal(Guid maintenancePlanId)
    {
        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);
        _maintenancePlanDefinitions.Remove(maintenancePlan!);
    }

    /// <summary>
    /// Internal method for finding a parameter. Should only be called by MaintenancePlanManagementService.
    /// </summary>
    public AssetModelParameter? FindParameter(Guid parameterId)
    {
        return _parameters.FirstOrDefault(p => p.ParameterId == parameterId);
    }

    /// <summary>
    /// Internal method for getting a maintenance plan. Should only be called by MaintenancePlanManagementService.
    /// </summary>
    public MaintenancePlanDefinition? GetMaintenancePlan(Guid maintenancePlanId)
    {
        return _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);
    }

    public void AddImage(Guid fileId, string filePath)
    {
        DomainGuard.AgainstBusinessRule(
            _images.Count >= MaxImagesPerModel,
            "MaxImagesLimit",
            $"Cannot exceed {MaxImagesPerModel} images per asset model");
        DomainGuard.AgainstDuplicate(
            _images.Any(img => img.FileId == fileId),
            nameof(AssetModelImage),
            nameof(fileId),
            fileId);

        var image = new AssetModelImage(fileId, filePath);
        _images.Add(image);

        // RaiseDomainEvent(new AssetModelImageAddedEvent(Id, fileId));
    }

    public void RemoveImage(Guid fileId)
    {
        var image = _images.FirstOrDefault(img => img.FileId == fileId);
        DomainGuard.AgainstNotFound(image, nameof(AssetModelImage), fileId);

        _images.Remove(image!);

        // RaiseDomainEvent(new AssetModelImageRemovedEvent(Id, fileId));
    }

    private void ValidateMaintenancePlanLimit()
    {
        DomainGuard.AgainstBusinessRule(
            _maintenancePlanDefinitions.Count >= MaxMaintenancePlansPerModel,
            "MaintenancePlanLimit",
            $"Cannot exceed {MaxMaintenancePlansPerModel} maintenance plans per asset model");
    }

    private void ValidateParentId(Guid? parentId)
    {
        if (!parentId.HasValue)
            return;

        DomainGuard.AgainstInvalidForeignKey(parentId.Value, nameof(ParentId));
        DomainGuard.AgainstInvalidState(
            parentId.Value == Id,
            nameof(AssetModel),
            $"ParentId={parentId.Value}",
            "Asset model cannot be its own parent");
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
