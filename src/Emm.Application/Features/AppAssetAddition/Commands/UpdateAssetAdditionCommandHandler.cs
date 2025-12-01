using Emm.Domain.Entities.AssetTransaction;

namespace Emm.Application.Features.AppAssetAddition.Commands;

public class UpdateAssetAdditionCommandHandler : IRequestHandler<UpdateAssetAdditionCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<AssetAddition, long> _assetAdditionRepository;

    public UpdateAssetAdditionCommandHandler(
        IUnitOfWork unitOfWork, 
        IRepository<AssetAddition, long> assetAdditionRepository)
    {
        _unitOfWork = unitOfWork;
        _assetAdditionRepository = assetAdditionRepository;
    }

    public async Task<Result<object>> Handle(UpdateAssetAdditionCommand request, CancellationToken cancellationToken)
    {
        var assetAddition = await _assetAdditionRepository.GetByIdAsync(request.Id);
        if (assetAddition == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "AssetAddition not found");
        }

        // Update AssetAddition properties
        // Note: You'll need to add update methods to AssetAddition entity (Aggregate Root)
        // All updates should go through the Aggregate Root methods

        // Handle AssetAdditionLines updates through Aggregate Root
        // 1. AssetAddition should have methods to manage its AssetAdditionLines
        // 2. All line operations (add/update/remove) should be done via AR methods
        // 3. EF Core will track changes in the aggregate automatically

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = assetAddition.Id
        });
    }
}
