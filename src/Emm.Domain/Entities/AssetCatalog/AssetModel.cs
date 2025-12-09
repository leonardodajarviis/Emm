using Emm.Domain.Abstractions;
using Emm.Domain.Exceptions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.AssetCatalog;

public class AssetModel : AggregateRoot, IAuditableEntity
{
    private const int MaxMaintenancePlansPerModel = 100;
    private const int MaxImagesPerModel = 50;
    private const int MaxParametersPerModel = 200;

    public long Id { get; private set; }
    public bool IsCodeGenerated { get; private set; }
    public NaturalKey Code { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? Notes { get; private set; }
    public long? ParentId { get; private set; }
    public long? AssetCategoryId { get; private set; }
    public long? AssetTypeId { get; private set; }
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
        string? description = null,
        string? notes = null,
        long? parentId = null,
        long? assetCategoryId = null,
        long? assetTypeId = null,
        bool isActive = true)
    {

        ValidateName(name);
        ValidateForeignKey(assetCategoryId, nameof(AssetCategoryId));
        ValidateForeignKey(assetTypeId, nameof(AssetTypeId));

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
        ValidateFileId(fileId);
        ValidateFileUrl(fileUrl);

        ThumbnailFileId = fileId;
        ThumbnailUrl = fileUrl;
    }

    public void UpdateThumbnail(Guid fileId, string fileUrl)
    {
        ValidateFileId(fileId);
        ValidateFileUrl(fileUrl);

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
        long? parentId,
        long? assetCategoryId,
        long? assetTypeId,
        bool isActive)
    {
        ValidateName(name);
        ValidateParentId(parentId);
        ValidateForeignKey(assetCategoryId, nameof(AssetCategoryId));
        ValidateForeignKey(assetTypeId, nameof(AssetTypeId));

        var hasChanges = Name != name
            || Description != description
            || Notes != notes
            || ParentId != parentId
            || AssetCategoryId != assetCategoryId
            || AssetTypeId != assetTypeId
            || IsActive != isActive;

        if (!hasChanges)
            return;

        Name = name;
        Description = description;
        Notes = notes;
        ParentId = parentId;
        AssetCategoryId = assetCategoryId;
        AssetTypeId = assetTypeId;
        IsActive = isActive;

        // RaiseDomainEvent(new AssetModelUpdatedEvent(Id, Name));
    }

    public void AddParameter(long parameterId)
    {
        if (parameterId <= 0)
            throw new DomainException("Parameter ID must be greater than zero");

        if (_parameters.Count >= MaxParametersPerModel)
            throw new DomainException($"Cannot exceed {MaxParametersPerModel} parameters per asset model");

        if (_parameters.Any(p => p.ParameterId == parameterId))
            throw new DomainException($"Parameter {parameterId} already exists in this asset model");

        var parameter = new AssetModelParameter(parameterId);
        _parameters.Add(parameter);

        // RaiseDomainEvent(new AssetModelParameterAddedEvent(Id, parameterId));
    }

    public void AddParameters(params long[] parameterIds)
    {
        if (parameterIds == null || parameterIds.Length == 0)
            return;

        var distinctIds = parameterIds.Distinct().ToArray();

        // Validate all IDs first
        foreach (var id in distinctIds)
        {
            if (id <= 0)
                throw new DomainException("Parameter ID must be greater than zero");
        }

        // Check limit
        var newParametersCount = distinctIds.Count(id => !_parameters.Any(p => p.ParameterId == id));
        if (_parameters.Count + newParametersCount > MaxParametersPerModel)
            throw new DomainException($"Cannot exceed {MaxParametersPerModel} parameters per asset model");

        // Add parameters by reusing AddParameter logic
        foreach (var id in distinctIds)
        {
            if (!_parameters.Any(p => p.ParameterId == id))
            {
                AddParameter(id);
            }
        }
    }

    public void RemoveParameter(long parameterId)
    {
        if (parameterId <= 0)
            throw new DomainException("Parameter ID must be greater than zero");

        var parameter = _parameters.FirstOrDefault(x => x.ParameterId == parameterId);

        if (parameter == null)
            throw new DomainException($"Parameter {parameterId} not found in this asset model");

        _parameters.Remove(parameter);

        // RaiseDomainEvent(new AssetModelParameterRemovedEvent(Id, parameterId));
    }

    public void RemoveParameters(IReadOnlyCollection<long> parameterIds, bool throwIfNotFound = false)
    {
        if (parameterIds == null || parameterIds.Count == 0)
            return;

        foreach (var parameterId in parameterIds)
        {
            if (parameterId <= 0)
                throw new DomainException("Parameter ID must be greater than zero");

            var parameter = _parameters.FirstOrDefault(x => x.ParameterId == parameterId);

            if (parameter == null)
            {
                if (throwIfNotFound)
                    throw new DomainException($"Parameter {parameterId} not found in this asset model");
                continue;
            }

            _parameters.Remove(parameter);
            // RaiseDomainEvent(new AssetModelParameterRemovedEvent(Id, parameterId));
        }
    }

    // Time-based maintenance plan methods
    public void AddTimeBasedMaintenancePlan(
        string name,
        string? description,
        string rrule,
        IReadOnlyCollection<MaintenancePlanJobStepDefinitionSpec>? jobSteps = null,
        IReadOnlyCollection<MaintenancePlanRequiredItemDefinitionSpec>? requiredItems = null,
        bool isActive = true)
    {
        ValidateMaintenancePlanLimit();
        ValidateMaintenancePlanName(name);

        var maintenancePlan = new MaintenancePlanDefinition(
            assetModelId: Id,
            name: name,
            description: description,
            rrule: rrule,
            jobSteps: jobSteps,
            requiredItems: requiredItems,
            isActive: isActive
        );

        _maintenancePlanDefinitions.Add(maintenancePlan);

        // RaiseDomainEvent(new MaintenancePlanAddedEvent(Id, maintenancePlan.Id, name));
    }

    // Parameter-based maintenance plan methods
    public void AddParameterBasedMaintenancePlan(
        string name,
        string? description,
        long parameterId,
        decimal triggerValue,
        decimal minValue,
        decimal maxValue,
        MaintenanceTriggerCondition triggerCondition = MaintenanceTriggerCondition.GreaterThanOrEqual,
        IReadOnlyCollection<MaintenancePlanJobStepDefinitionSpec>? jobSteps = null,
        IReadOnlyCollection<MaintenancePlanRequiredItemDefinitionSpec>? requiredItems = null,
        bool isActive = true)
    {
        ValidateMaintenancePlanLimit();
        ValidateMaintenancePlanName(name);

        var maintenancePlan = new MaintenancePlanDefinition(
            assetModelId: Id,
            name: name,
            description: description,
            parameterId: parameterId,
            triggerValue: triggerValue,
            minValue: minValue,
            maxValue: maxValue,
            triggerCondition: triggerCondition,
            jobSteps: jobSteps,
            requiredItems: requiredItems,
            isActive: isActive
        );

        _maintenancePlanDefinitions.Add(maintenancePlan);

        // RaiseDomainEvent(new MaintenancePlanAddedEvent(Id, maintenancePlan.Id, name));
    }

    // General maintenance plan methods
    public void RemoveMaintenancePlan(long maintenancePlanId)
    {
        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);

        if (maintenancePlan == null)
            throw new DomainException($"Maintenance plan {maintenancePlanId} not found in this asset model");

        _maintenancePlanDefinitions.Remove(maintenancePlan);

        // RaiseDomainEvent(new MaintenancePlanRemovedEvent(Id, maintenancePlanId));
    }

    public void UpdateMaintenancePlan(
        long maintenancePlanId,
        string name,
        string? description,
        bool isActive)
    {
        ValidateMaintenancePlanName(name);

        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);

        if (maintenancePlan == null)
            throw new DomainException($"Maintenance plan {maintenancePlanId} not found in this asset model");

        maintenancePlan.Update(name, description, isActive);

        // RaiseDomainEvent(new MaintenancePlanUpdatedEvent(Id, maintenancePlanId, name));
    }

    public void UpdateTimeBasedMaintenancePlan(
        long maintenancePlanId,
        string name,
        string? description,
        string rrule,
        bool isActive)
    {
        ValidateMaintenancePlanName(name);

        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);

        if (maintenancePlan == null)
            throw new DomainException($"Maintenance plan {maintenancePlanId} not found in this asset model");

        maintenancePlan.UpdateTimeBasedPlan(name, description, rrule, isActive);

        // RaiseDomainEvent(new MaintenancePlanUpdatedEvent(Id, maintenancePlanId, name));
    }

    public void UpdateParameterBasedMaintenancePlan(
        long maintenancePlanId,
        string name,
        string? description,
        decimal triggerValue,
        decimal minValue,
        decimal maxValue,
        MaintenanceTriggerCondition triggerCondition,
        bool isActive)
    {
        ValidateMaintenancePlanName(name);

        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);

        if (maintenancePlan == null)
            throw new DomainException($"Maintenance plan {maintenancePlanId} not found in this asset model");

        maintenancePlan.UpdateParameterBasedPlan(
            name,
            description,
            triggerValue,
            minValue,
            maxValue,
            triggerCondition,
            isActive
        );

        // RaiseDomainEvent(new MaintenancePlanUpdatedEvent(Id, maintenancePlanId, name));
    }

    // Job steps management methods
    public void AddJobStepToMaintenancePlan(
        long maintenancePlanId,
        string stepName,
        long? organizationUnitId,
        string? note,
        int order)
    {
        ValidateMaintenancePlanName(stepName);

        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);

        if (maintenancePlan == null)
            throw new DomainException($"Maintenance plan {maintenancePlanId} not found in this asset model");

        maintenancePlan.AddJobStep(stepName, organizationUnitId, note, order);

        // RaiseDomainEvent(new MaintenancePlanJobStepAddedEvent(Id, maintenancePlanId, stepName));
    }

    public void RemoveJobStepFromMaintenancePlan(
        long maintenancePlanId,
        long jobStepId)
    {
        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);

        if (maintenancePlan == null)
            throw new DomainException($"Maintenance plan {maintenancePlanId} not found in this asset model");

        maintenancePlan.RemoveJobStep(jobStepId);

        // RaiseDomainEvent(new MaintenancePlanJobStepRemovedEvent(Id, maintenancePlanId, jobStepId));
    }

    public void UpdateJobStepInMaintenancePlan(
        long maintenancePlanId,
        long jobStepId,
        string stepName,
        string? note,
        int order)
    {
        ValidateMaintenancePlanName(stepName);

        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);

        if (maintenancePlan == null)
            throw new DomainException($"Maintenance plan {maintenancePlanId} not found in this asset model");

        // Delegate to MaintenancePlanDefinition to respect aggregate boundaries
        maintenancePlan.UpdateJobStep(jobStepId, stepName, note, order);

        // RaiseDomainEvent(new MaintenancePlanJobStepUpdatedEvent(Id, maintenancePlanId, jobStepId, stepName));
    }

    // Required items management methods
    public void AddRequiredItemToMaintenancePlan(
        long maintenancePlanId,
        long itemId,
        decimal quantity,
        bool isRequired,
        string? note = null)
    {
        if (itemId <= 0)
            throw new DomainException("Item ID must be greater than zero");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);

        if (maintenancePlan == null)
            throw new DomainException($"Maintenance plan {maintenancePlanId} not found in this asset model");

        maintenancePlan.AddRequiredItem(itemId, quantity, isRequired, note);

        // RaiseDomainEvent(new MaintenancePlanRequiredItemAddedEvent(Id, maintenancePlanId, itemId));
    }

    public void RemoveRequiredItemFromMaintenancePlan(
        long maintenancePlanId,
        long requiredItemId)
    {
        if (requiredItemId <= 0)
            throw new DomainException("Required item ID must be greater than zero");

        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);

        if (maintenancePlan == null)
            throw new DomainException($"Maintenance plan {maintenancePlanId} not found in this asset model");

        maintenancePlan.RemoveRequiredItem(requiredItemId);

        // RaiseDomainEvent(new MaintenancePlanRequiredItemRemovedEvent(Id, maintenancePlanId, requiredItemId));
    }

    public void UpdateRequiredItemInMaintenancePlan(
        long maintenancePlanId,
        long requiredItemId,
        decimal quantity,
        bool isRequired,
        string? note)
    {
        if (requiredItemId <= 0)
            throw new DomainException("Required item ID must be greater than zero");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);

        if (maintenancePlan == null)
            throw new DomainException($"Maintenance plan {maintenancePlanId} not found in this asset model");

        maintenancePlan.UpdateRequiredItem(requiredItemId, quantity, isRequired, note);

        // RaiseDomainEvent(new MaintenancePlanRequiredItemUpdatedEvent(Id, maintenancePlanId, requiredItemId));
    }

    public void SyncRequiredItemsInMaintenancePlan(
        long maintenancePlanId,
        IReadOnlyCollection<RequiredItemSpec> requiredItemSpecs)
    {
        if (requiredItemSpecs == null)
            throw new DomainException("Required items specs cannot be null");

        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);

        if (maintenancePlan == null)
            throw new DomainException($"Maintenance plan {maintenancePlanId} not found in this asset model");

        maintenancePlan.SyncRequiredItems(requiredItemSpecs);

        // RaiseDomainEvent(new MaintenancePlanRequiredItemsSyncedEvent(Id, maintenancePlanId));
    }

    public void AddImage(Guid fileId, string filePath)
    {
        ValidateFileId(fileId);
        ValidateFilePath(filePath);

        if (_images.Count >= MaxImagesPerModel)
            throw new DomainException($"Cannot exceed {MaxImagesPerModel} images per asset model");

        if (_images.Any(img => img.FileId == fileId))
            throw new DomainException($"Image with file ID {fileId} already exists in this asset model");

        var image = new AssetModelImage(fileId, filePath);
        _images.Add(image);

        // RaiseDomainEvent(new AssetModelImageAddedEvent(Id, fileId));
    }

    public void RemoveImage(Guid fileId)
    {
        ValidateFileId(fileId);

        var image = _images.FirstOrDefault(img => img.FileId == fileId);

        if (image == null)
            throw new DomainException($"Image with file ID {fileId} not found in this asset model");

        _images.Remove(image);

        // RaiseDomainEvent(new AssetModelImageRemovedEvent(Id, fileId));
    }

    // Validation methods
    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Asset model code cannot be empty");

        if (code.Length > 50)
            throw new DomainException("Asset model code cannot exceed 50 characters");
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Asset model name cannot be empty");

        if (name.Length > 200)
            throw new DomainException("Asset model name cannot exceed 200 characters");
    }

    private static void ValidateFileId(Guid fileId)
    {
        if (fileId == Guid.Empty)
            throw new DomainException("File ID cannot be empty");
    }

    private static void ValidateFileUrl(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            throw new DomainException("File URL cannot be empty");
    }

    private static void ValidateFilePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new DomainException("File path cannot be empty");
    }

    private void ValidateMaintenancePlanLimit()
    {
        if (_maintenancePlanDefinitions.Count >= MaxMaintenancePlansPerModel)
            throw new DomainException($"Cannot exceed {MaxMaintenancePlansPerModel} maintenance plans per asset model");
    }

    private static void ValidateMaintenancePlanName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Maintenance plan name cannot be empty");

        if (name.Length > 200)
            throw new DomainException("Maintenance plan name cannot exceed 200 characters");
    }

    private void ValidateParentId(long? parentId)
    {
        if (!parentId.HasValue)
            return;

        if (parentId.Value <= 0)
            throw new DomainException("Parent ID must be greater than zero");

        if (parentId.Value == Id)
            throw new DomainException("Asset model cannot be its own parent");
    }

    private static void ValidateForeignKey(long? foreignKeyId, string fieldName)
    {
        if (!foreignKeyId.HasValue)
            return;

        if (foreignKeyId.Value <= 0)
            throw new DomainException($"{fieldName} must be greater than zero");
    }
}

public class AssetModelParameter
{
    public long AssetModelId { get; private set; }
    public long ParameterId { get; private set; }

    public AssetModelParameter(long parameterId)
    {
        ParameterId = parameterId;
    }

    private AssetModelParameter()
    {
    } // EF Core constructor
}

public class AssetModelImage
{
    public long AssetModelId { get; private set; }
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
