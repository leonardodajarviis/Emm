using Emm.Domain.Abstractions;
using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAsset.Commands;

public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Asset, long> _repository;

    public CreateAssetCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<Asset, long> repository)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(repository);

        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = new Asset(
            code: request.Code,
            displayName: request.DisplayName,
            assetModelId: request.AssetModelId,
            organizationUnitId: 1, // TODO: get from user context
            locationId: 1, // TODO: get from user context
            assetAdditionId: null,
            description: request.Description
        );

        await _repository.AddAsync(asset);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = asset.Id
        });
    }
}
