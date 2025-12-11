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
        DomainGuard.AgainstNegativeOrZero(parameterId, nameof(parameterId));
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

    public void AddParameters(params long[] parameterIds)
    {
        if (parameterIds == null || parameterIds.Length == 0)
            return;

        var distinctIds = parameterIds.Distinct().ToArray();

        // Validate all IDs first
        foreach (var id in distinctIds)
        {
            DomainGuard.AgainstNegativeOrZero(id, "ParameterId");
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

    public void RemoveParameter(long parameterId)
    {
        DomainGuard.AgainstNegativeOrZero(parameterId, nameof(parameterId));

        var parameter = _parameters.FirstOrDefault(x => x.ParameterId == parameterId);
        DomainGuard.AgainstNotFound(parameter, nameof(AssetModelParameter), parameterId);

        _parameters.Remove(parameter!);

        // RaiseDomainEvent(new AssetModelParameterRemovedEvent(Id, parameterId));
    }

    public void RemoveParameters(IReadOnlyCollection<long> parameterIds, bool throwIfNotFound = false)
    {
        if (parameterIds == null || parameterIds.Count == 0)
            return;

        foreach (var parameterId in parameterIds)
        {
            DomainGuard.AgainstNegativeOrZero(parameterId, nameof(parameterId));

            var parameter = _parameters.FirstOrDefault(x => x.ParameterId == parameterId);

            if (parameter == null)
            {
                if (throwIfNotFound)
                    DomainGuard.AgainstNotFound(parameter, nameof(AssetModelParameter), parameterId);
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

        var existParameter = _parameters.FirstOrDefault(p => p.ParameterId == parameterId);
        if (existParameter == null)
        {
            throw new DomainException($"Parameter {parameterId} is not associated with this AssetModel");
        }

        existParameter.MarkAsMaintenanceParameter();

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
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        if (maintenancePlan!.PlanType == MaintenancePlanType.ParameterBased)
        {
            if (maintenancePlan.ParameterBasedTrigger == null)
            {
                throw new DomainException("ParameterBasedTrigger is null for a parameter-based maintenance plan");
            }

            var parameterId = maintenancePlan.ParameterBasedTrigger.ParameterId;
            var associatedParameter = _parameters.FirstOrDefault(p => p.ParameterId == parameterId);

            associatedParameter?.UnmarkAsMaintenanceParameter();
        }

        _maintenancePlanDefinitions.Remove(maintenancePlan!);

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
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.Update(name, description, isActive);

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
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.UpdateTimeBasedPlan(name, description, rrule, isActive);

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
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.UpdateParameterBasedPlan(
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
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.AddJobStep(stepName, organizationUnitId, note, order);

        // RaiseDomainEvent(new MaintenancePlanJobStepAddedEvent(Id, maintenancePlanId, stepName));
    }

    public void RemoveJobStepFromMaintenancePlan(
        long maintenancePlanId,
        long jobStepId)
    {
        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.RemoveJobStep(jobStepId);

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
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        // Delegate to MaintenancePlanDefinition to respect aggregate boundaries
        maintenancePlan!.UpdateJobStep(jobStepId, stepName, note, order);

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
        DomainGuard.AgainstNegativeOrZero(itemId, nameof(itemId));
        DomainGuard.AgainstNegative(quantity, nameof(quantity));

        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.AddRequiredItem(itemId, quantity, isRequired, note);

        // RaiseDomainEvent(new MaintenancePlanRequiredItemAddedEvent(Id, maintenancePlanId, itemId));
    }

    public void RemoveRequiredItemFromMaintenancePlan(
        long maintenancePlanId,
        long requiredItemId)
    {
        DomainGuard.AgainstNegativeOrZero(requiredItemId, nameof(requiredItemId));

        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.RemoveRequiredItem(requiredItemId);

        // RaiseDomainEvent(new MaintenancePlanRequiredItemRemovedEvent(Id, maintenancePlanId, requiredItemId));
    }

    public void UpdateRequiredItemInMaintenancePlan(
        long maintenancePlanId,
        long requiredItemId,
        decimal quantity,
        bool isRequired,
        string? note)
    {
        DomainGuard.AgainstNegativeOrZero(requiredItemId, nameof(requiredItemId));
        DomainGuard.AgainstNegative(quantity, nameof(quantity));

        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.UpdateRequiredItem(requiredItemId, quantity, isRequired, note);

        // RaiseDomainEvent(new MaintenancePlanRequiredItemUpdatedEvent(Id, maintenancePlanId, requiredItemId));
    }

    public void SyncRequiredItemsInMaintenancePlan(
        long maintenancePlanId,
        IReadOnlyCollection<RequiredItemSpec> requiredItemSpecs)
    {
        DomainGuard.AgainstNull(requiredItemSpecs, nameof(requiredItemSpecs));

        var maintenancePlan = _maintenancePlanDefinitions.FirstOrDefault(mp => mp.Id == maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.SyncRequiredItems(requiredItemSpecs);

        // RaiseDomainEvent(new MaintenancePlanRequiredItemsSyncedEvent(Id, maintenancePlanId));
    }

    public void AddImage(Guid fileId, string filePath)
    {
        ValidateFileId(fileId);
        ValidateFilePath(filePath);

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
        ValidateFileId(fileId);

        var image = _images.FirstOrDefault(img => img.FileId == fileId);
        DomainGuard.AgainstNotFound(image, nameof(AssetModelImage), fileId);

        _images.Remove(image!);

        // RaiseDomainEvent(new AssetModelImageRemovedEvent(Id, fileId));
    }

    // Validation methods
    private static void ValidateCode(string code)
    {
        DomainGuard.AgainstNullOrEmpty(code, nameof(Code));
        DomainGuard.AgainstTooLong(code, 50, nameof(Code));
    }

    private static void ValidateName(string name)
    {
        DomainGuard.AgainstNullOrEmpty(name, nameof(Name));
        DomainGuard.AgainstTooLong(name, 200, nameof(Name));
    }

    private static void ValidateFileId(Guid fileId)
    {
        DomainGuard.AgainstDefault(fileId, "FileId");
    }

    private static void ValidateFileUrl(string fileUrl)
    {
        DomainGuard.AgainstNullOrEmpty(fileUrl, "FileUrl");
    }

    private static void ValidateFilePath(string filePath)
    {
        DomainGuard.AgainstNullOrEmpty(filePath, "FilePath");
    }

    private void ValidateMaintenancePlanLimit()
    {
        DomainGuard.AgainstBusinessRule(
            _maintenancePlanDefinitions.Count >= MaxMaintenancePlansPerModel,
            "MaintenancePlanLimit",
            $"Cannot exceed {MaxMaintenancePlansPerModel} maintenance plans per asset model");
    }

    private static void ValidateMaintenancePlanName(string name)
    {
        DomainGuard.AgainstNullOrEmpty(name, "MaintenancePlanName");
        DomainGuard.AgainstTooLong(name, 200, "MaintenancePlanName");
    }

    private void ValidateParentId(long? parentId)
    {
        if (!parentId.HasValue)
            return;

        DomainGuard.AgainstNegativeOrZero(parentId.Value, nameof(ParentId));
        DomainGuard.AgainstInvalidState(
            parentId.Value == Id,
            nameof(AssetModel),
            $"ParentId={parentId.Value}",
            "Asset model cannot be its own parent");
    }

    private static void ValidateForeignKey(long? foreignKeyId, string fieldName)
    {
        if (!foreignKeyId.HasValue)
            return;

        DomainGuard.AgainstNegativeOrZero(foreignKeyId.Value, fieldName);
    }
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
