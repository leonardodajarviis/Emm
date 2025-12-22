using Emm.Application.ErrorCodes;
using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetCategory.Commands;

public class UpdateAssetCategoryCommandHandler : IRequestHandler<UpdateAssetCategoryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<AssetCategory, Guid> _repository;

    public UpdateAssetCategoryCommandHandler(IUnitOfWork unitOfWork, IRepository<AssetCategory, Guid> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(UpdateAssetCategoryCommand request, CancellationToken cancellationToken)
    {
        var updateBody = request.Body;
        var assetCategory = await _repository.GetByIdAsync(request.Id);
        if (assetCategory == null)
        {
            return Result.Failure(ErrorType.NotFound, "AssetCategory not found", AssetCategoryErrorCodes.NotFound);
        }

        assetCategory.Update(
            name: updateBody.Name,
            description: updateBody.Description,
            isActive: updateBody.IsActive
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
