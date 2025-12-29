using Emm.Application.Abstractions;
using Emm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAssetModel.Commands;

public class ChangeThumbnailCommandHandler : IRequestHandler<ChangeThumbnailCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetModelRepository _repository;
    private readonly IQueryContext _qq;
    private readonly IFileStorage _fileStorage;

    public ChangeThumbnailCommandHandler(
        IUnitOfWork unitOfWork,
        IAssetModelRepository repository,
        IQueryContext queryContext,
        IFileStorage fileStorage)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _qq = queryContext;
        _fileStorage = fileStorage;
    }

    public async Task<Result> Handle(ChangeThumbnailCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // Get the AssetModel
            var assetModel = await _repository.GetByIdAsync(request.AssetModelId, cancellationToken);

            if (assetModel == null)
            {
                return Result.Failure(ErrorType.NotFound, $"Asset model with ID {request.AssetModelId} not found.");
            }

            // Verify that the file exists in uploaded files
            var uploadedFile = await _qq.Query<UploadedFile>()
                .FirstOrDefaultAsync(f => f.Id == request.FileId, cancellationToken);

            if (uploadedFile == null)
            {
                return Result.Failure(ErrorType.NotFound, $"Uploaded file with ID {request.FileId} not found.");
            }

            // Check if the new thumbnail fileId exists in the current images
            var imageToRemove = assetModel.Images.FirstOrDefault(img => img.FileId == request.FileId);
            if (imageToRemove != null)
            {
                // Remove the image from the images collection
                assetModel.RemoveImage(request.FileId);
            }

            // Update the thumbnail
            assetModel.UpdateThumbnail(uploadedFile.Id, uploadedFile.FilePath);

            // Mark the file as used
            await _fileStorage.FileUsedAsync(uploadedFile.Id, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        });
    }
}
