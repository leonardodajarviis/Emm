using Emm.Application.Abstractions;
using Emm.Domain.Entities;
using Emm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAssetModel.Commands;

public class AddImagesToAssetModelCommandHandler : IRequestHandler<AddImagesToAssetModelCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetModelRepository _repository;
    private readonly IFileStorage _fileStorage;
    private readonly IQueryContext _queryContext;

    public AddImagesToAssetModelCommandHandler(
        IAssetModelRepository repository,
        IUnitOfWork unitOfWork,
        IFileStorage fileStorage,
        IQueryContext queryContext)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(fileStorage);
        ArgumentNullException.ThrowIfNull(queryContext);

        _unitOfWork = unitOfWork;
        _repository = repository;
        _fileStorage = fileStorage;
        _queryContext = queryContext;
    }

    public async Task<Result<object>> Handle(AddImagesToAssetModelCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var assetModel = await _repository.GetByIdAsync(request.AssetModelId, cancellationToken);
        if (assetModel == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, $"Asset model with ID {request.AssetModelId} not found.");
        }

        var addedFileIds = new List<Guid>();

        foreach (var fileId in request.FileIds)
        {
            var uploadedFile = await _queryContext.Query<UploadedFile>()
                .FirstOrDefaultAsync(f => f.Id == fileId, cancellationToken);

            if (uploadedFile == null)
            {
                return Result<object>.Failure(ErrorType.NotFound, $"Uploaded file with ID {fileId} not found.");
            }

            assetModel.AddImage(fileId, uploadedFile.FilePath);
            await _fileStorage.FileUsedAsync(fileId, cancellationToken);
            addedFileIds.Add(fileId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new { assetModel.Id, FileIds = addedFileIds });
    }
}
