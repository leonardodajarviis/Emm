using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAssetType.Commands;

public class CreateAssetTypeCommandHandler : IRequestHandler<CreateAssetTypeCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetTypeRepository _repository;
    private readonly IQueryContext _queryContext;

    public CreateAssetTypeCommandHandler(IUnitOfWork unitOfWork, IAssetTypeRepository repository, IQueryContext queryContext)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _queryContext = queryContext;
    }

    public async Task<Result<object>> Handle(CreateAssetTypeCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var code = await _unitOfWork.GenerateNextCodeAsync("LTS", "AssetTypes", 6, cancellationToken: cancellationToken);
            var assetType = new AssetType(
                code: code,
                name: request.Name,
                assetCategoryId: request.AssetCategoryId,
                description: request.Description,
                isActive: request.IsActive
            );

            assetType.AddParameters(request.ParameterIds);

            await _repository.AddAsync(assetType);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new
            {
                Id = assetType.Id
            });

        });
    }
}
