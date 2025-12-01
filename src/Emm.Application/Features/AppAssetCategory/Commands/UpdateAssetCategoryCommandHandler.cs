using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetCategory.Commands;

public class UpdateAssetCategoryCommandHandler : IRequestHandler<UpdateAssetCategoryCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<AssetCategory, long> _repository;

    public UpdateAssetCategoryCommandHandler(IUnitOfWork unitOfWork, IRepository<AssetCategory, long> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(UpdateAssetCategoryCommand request, CancellationToken cancellationToken)
    {
        var assetCategory = await _repository.GetByIdAsync(request.Id);
        if (assetCategory == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "AssetCategory not found");
        }

        assetCategory.Update(
            name: request.Name,
            description: request.Description,
            isActive: request.IsActive
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = assetCategory.Id
        });
    }
}
