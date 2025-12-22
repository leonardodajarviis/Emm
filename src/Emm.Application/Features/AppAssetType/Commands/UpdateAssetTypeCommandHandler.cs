
using Emm.Application.ErrorCodes;

namespace Emm.Application.Features.AppAssetType.Commands;

public class UpdateAssetTypeCommandHandler : IRequestHandler<UpdateAssetTypeCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetTypeRepository _repository;

    public UpdateAssetTypeCommandHandler(IUnitOfWork unitOfWork, IAssetTypeRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(UpdateAssetTypeCommand request, CancellationToken cancellationToken)
    {
        var assetType = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (assetType == null)
        {
            return Result.Failure(ErrorType.NotFound, "Asset type not found.", AssetTypeErrorCodes.NotFound);
        }

        assetType.Update(
            name: request.Body.Name,
            assetCategoryId: request.Body.AssetCategoryId,
            description: request.Body.Description,
            isActive: request.Body.IsActive
        );



        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
