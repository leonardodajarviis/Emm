using Emm.Application.Abstractions;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Services;
using Emm.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAssetModel.Commands;

public class CreateAssetModelCommandHandler : IRequestHandler<CreateAssetModelCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetModelRepository _repository;
    private readonly IQueryContext _qq;
    private readonly IFileStorage _fileStorage;
    private readonly IUserContextService _userContextService;
    private readonly ICodeGenerator _codeGenerator;
    // private readonly MaintenancePlanManagementService _maintenancePlanService;

    public CreateAssetModelCommandHandler(
        IUnitOfWork unitOfWork,
        IAssetModelRepository repository,
        IQueryContext queryContext,
        IFileStorage fileStorage,
        ICodeGenerator codeGenerator,
        IUserContextService userContextService
    )
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(queryContext);
        ArgumentNullException.ThrowIfNull(userContextService);
        ArgumentNullException.ThrowIfNull(codeGenerator);

        _unitOfWork = unitOfWork;
        _repository = repository;
        _fileStorage = fileStorage;
        _qq = queryContext;
        _userContextService = userContextService;
        _codeGenerator = codeGenerator;
        // _maintenancePlanService = maintenancePlanService;
    }

    public async Task<Result<object>> Handle(CreateAssetModelCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            NaturalKey code = new();

            if (request.IsCodeGenerated)
            {
                code = await _codeGenerator.GetNaturalKeyAsync<AssetModel>("DTB", 6, cancellationToken);
            }
            else
            {
                code = NaturalKey.CreateRaw(request.Code);
            }

            var existsCode = await _qq.Query<AssetModel>()
                .AnyAsync(am => am.Code == code, cancellationToken);

            if (existsCode)
            {
                return Result<object>.Failure(ErrorType.Conflict, $"Asset model with code {code} already exists.");
            }

            var assetModel = new AssetModel(
                isCodeGenerated: request.IsCodeGenerated,
                code: code,
                name: request.Name,
                description: request.Description,
                notes: request.Notes,
                parentId: request.ParentId,
                assetCategoryId: request.AssetCategoryId,
                assetTypeId: request.AssetTypeId,
                isActive: request.IsActive
            );

            var currentUserId = _userContextService.GetCurrentUserId();

            // Add parameters from AssetType if AssetTypeId is provided
            var assetParameterIds = await _qq.Query<AssetTypeParameter>()
                .Where(at => at.AssetTypeId == request.AssetTypeId)
                .Select(at => at.ParameterId).ToArrayAsync(cancellationToken);

            assetModel.AddParameters(assetParameterIds);

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
