using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetType.Commands;

public class UpdateAssetTypeCommandHandler : IRequestHandler<UpdateAssetTypeCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<AssetType, long> _repository;

    public UpdateAssetTypeCommandHandler(IUnitOfWork unitOfWork, IRepository<AssetType, long> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(UpdateAssetTypeCommand request, CancellationToken cancellationToken)
    {
        var assetType = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (assetType == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Asset type not found.");
        }

        assetType.Update(
            name: request.Name,
            assetCategoryId: request.AssetCategoryId,
            description: request.Description,
            isActive: request.IsActive
        );
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<object>.Success(new
        {
            Id = request.Id
        });
    }
}
