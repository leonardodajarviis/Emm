namespace Emm.Application.Features.AppAssetModel.Commands;


public class RemoveParametersFromAssetModelCommandHandler : IRequestHandler<RemoveParametersFromAssetModelCommand, Result<object>>
{
    private readonly IAssetModelRepository _assetModelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveParametersFromAssetModelCommandHandler(IAssetModelRepository assetModelRepository, IUnitOfWork unitOfWork)
    {
        _assetModelRepository = assetModelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<object>> Handle(RemoveParametersFromAssetModelCommand request, CancellationToken cancellationToken)
    {
        var assetModel = await _assetModelRepository.GetByIdAsync(request.AssetModelId, cancellationToken);
        if (assetModel == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Asset model not found.");
        }

        assetModel.RemoveParameters(request.ParameterIds);
        _assetModelRepository.Update(assetModel);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new {success = true});
    }
}
