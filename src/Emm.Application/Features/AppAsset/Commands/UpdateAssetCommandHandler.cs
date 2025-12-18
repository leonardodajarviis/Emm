using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAsset.Commands;

public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Asset, Guid> _repository;

    public UpdateAssetCommandHandler(IUnitOfWork unitOfWork, IRepository<Asset, Guid> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await _repository.GetByIdAsync(request.Id);
        if (asset == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Asset not found");
        }

        asset.Update(
            displayName: request.DisplayName,
            description: request.Description,
            organizationUnitId: asset.OrganizationUnitId, // Keep current value
            locationId: asset.LocationId // Keep current value
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = asset.Id
        });
    }
}
