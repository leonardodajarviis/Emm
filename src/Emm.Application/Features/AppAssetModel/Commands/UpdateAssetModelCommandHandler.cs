using Emm.Domain.Abstractions;
using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetModel.Commands;

public class UpdateAssetModelCommandHandler : IRequestHandler<UpdateAssetModelCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<AssetModel, Guid> _repository;

    public UpdateAssetModelCommandHandler(IUnitOfWork unitOfWork, IRepository<AssetModel, Guid> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(UpdateAssetModelCommand request, CancellationToken cancellationToken)
    {
        var assetModel = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (assetModel == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Asset model not found.");
        }

        assetModel.Update(
            name: request.Name,
            description: request.Description,
            notes: request.Notes,
            parentId: request.ParentId,
            assetCategoryId: request.AssetCategoryId,
            assetTypeId: request.AssetTypeId,
            isActive: request.IsActive
        );
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<object>.Success(new
        {
            Id = request.Id
        });
    }
}
