using Emm.Application.Abstractions;
using Emm.Domain.Repositories;

namespace Emm.Application.Features.AppAssetModel.Commands;

public class RemoveImagesFromAssetModelCommandHandler : IRequestHandler<RemoveImagesFromAssetModelCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetModelRepository _repository;

    public RemoveImagesFromAssetModelCommandHandler(
        IAssetModelRepository repository,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(repository);

        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(RemoveImagesFromAssetModelCommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var assetModel = await _repository.GetByIdAsync(request.AssetModelId, cancellationToken);
        if (assetModel == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, $"Asset model with ID {request.AssetModelId} not found.");
        }

        foreach (var fileId in request.FileIds)
        {
            assetModel.RemoveImage(fileId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new { assetModel.Id, FileIds = request.FileIds });
    }
}
