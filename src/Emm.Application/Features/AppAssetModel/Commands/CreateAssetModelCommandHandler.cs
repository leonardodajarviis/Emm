using Emm.Application.Abstractions;
using Emm.Domain.Abstractions;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAssetModel.Commands;

public class CreateAssetModelCommandHandler : IRequestHandler<CreateAssetModelCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetModelRepository _repository;
    private readonly IQueryContext _qq;
    private readonly IFileStorage _fileStorage;

    public CreateAssetModelCommandHandler(
        IUnitOfWork unitOfWork,
        IAssetModelRepository repository,
        IQueryContext queryContext,
        IFileStorage fileStorage)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(queryContext);

        _unitOfWork = unitOfWork;
        _repository = repository;
        _fileStorage = fileStorage;
        _qq = queryContext;
    }

    public async Task<Result<object>> Handle(CreateAssetModelCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var code = await _unitOfWork.GenerateNextCodeAsync("DTS", "AssetModels", 6, cancellationToken);
            var assetModel = new AssetModel(
                code: code,
                name: request.Name,
                description: request.Description,
                notes: request.Notes,
                parentId: request.ParentId,
                assetCategoryId: request.AssetCategoryId,
                assetTypeId: request.AssetTypeId,
                isActive: request.IsActive
            );

            // Add parameters from AssetType if AssetTypeId is provided
            if (request.AssetTypeId.HasValue)
            {
                var assetParameterIds = await _qq.Query<AssetTypeParameter>()
                    .Where(at => at.AssetTypeId == request.AssetTypeId)
                    .Select(at => at.ParameterId).ToArrayAsync(cancellationToken);

                assetModel.AddParameters(assetParameterIds);
            }

            // Add additional parameters if provided
            if (request.ParameterIds?.Count > 0)
            {
                assetModel.AddParameters([.. request.ParameterIds]);
            }

            // Add maintenance plan definitions
            if (request.MaintenancePlanDefinitions?.Count > 0)
            {
                AddMaintenancePlanDefinitions(assetModel, request.MaintenancePlanDefinitions);
            }

            // Add images
            if (request.Images?.Count > 0)
            {
                await AddImages(assetModel, request.Images, cancellationToken);
            }

            // Set thumbnail if provided
            if (request.ThumbnailFileId.HasValue)
            {
                var uploadedFile = await _qq.Query<UploadedFile>()
                    .FirstOrDefaultAsync(f => f.Id == request.ThumbnailFileId.Value, cancellationToken);

                if (uploadedFile == null)
                {
                    return Result<object>.Failure(ErrorType.Conflict, $"Uploaded file with ID {request.ThumbnailFileId} not found.");
                }

                assetModel.SetThumbnailOnCreate(uploadedFile.Id, uploadedFile.FilePath);
                await _fileStorage.FileUsedAsync(uploadedFile.Id, cancellationToken);
            }

            await _repository.AddAsync(assetModel, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new
            {
                assetModel.Id,
            });
        });
    }

    private static void AddMaintenancePlanDefinitions(AssetModel assetModel, IReadOnlyCollection<CreateMaintenancePlanDefinitionCommand> maintenancePlanDefinitions)
    {
        foreach (var definition in maintenancePlanDefinitions)
        {
            var jobSteps = definition.JobSteps?.Select(js => new MaintenancePlanJobStepDefinitionSpec(
                Name: js.Name,
                OrganizationUnitId: js.OrganizationUnitId,
                Note: js.Note,
                Order: js.Order)).ToList() ?? [];

            switch (definition.PlanType)
            {
                case MaintenancePlanType.TimeBased:
                    assetModel.AddTimeBasedMaintenancePlan(
                        name: definition.Name,
                        description: definition.Description,
                        rrule: definition.RRule ?? string.Empty,
                        jobSteps: jobSteps,
                        isActive: definition.IsActive
                    );
                    break;

                case MaintenancePlanType.ParameterBased:
                    if (!definition.ParameterId.HasValue || !definition.TriggerValue.HasValue ||
                        !definition.MinValue.HasValue || !definition.MaxValue.HasValue)
                    {
                        throw new ArgumentException("ParameterId, TriggerValue, MinValue, and MaxValue are required for parameter-based maintenance plans");
                    }

                    assetModel.AddParameterBasedMaintenancePlan(
                        name: definition.Name,
                        description: definition.Description,
                        parameterId: definition.ParameterId.Value,
                        triggerValue: definition.TriggerValue.Value,
                        minValue: definition.MinValue.Value,
                        maxValue: definition.MaxValue.Value,
                        triggerCondition: definition.TriggerCondition ?? MaintenanceTriggerCondition.GreaterThanOrEqual,
                        jobSteps: jobSteps,
                        isActive: definition.IsActive
                    );
                    break;

                default:
                    // For general maintenance plans with job steps
                    assetModel.AddMaintenancePlanWithJobSteps(
                        name: definition.Name,
                        description: definition.Description,
                        planType: definition.PlanType,
                        jobSteps: jobSteps,
                        isActive: definition.IsActive);
                    break;
            }
        }
    }

    private async Task AddImages(AssetModel assetModel, IReadOnlyCollection<CreateAssetModelImageCommand> images, CancellationToken cancellationToken)
    {
        var imageGuids = images.Select(i => i.FileId).ToList();
        var existingFiles = await _qq.Query<UploadedFile>()
            .Where(f => imageGuids.Contains(f.Id))
            .ToDictionaryAsync(f => f.Id, cancellationToken);

        foreach (var image in images)
        {
            var uploadedFile = existingFiles.GetValueOrDefault(image.FileId) ?? throw new ArgumentException($"Uploaded file with ID {image.FileId} not found.");

            assetModel.AddImage(image.FileId, uploadedFile.FilePath);
            await _fileStorage.FileUsedAsync(image.FileId, cancellationToken);
        }
    }
}
