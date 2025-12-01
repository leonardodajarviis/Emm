using Emm.Application.Abstractions;
using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetCategory.Commands;

public class CreateAssetCategoryCommandHandler : IRequestHandler<CreateAssetCategoryCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<AssetCategory, long> _repository;
    private readonly IOutbox _outbox;

    public CreateAssetCategoryCommandHandler(IUnitOfWork unitOfWork, IRepository<AssetCategory, long> repository, IOutbox outbox)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _outbox = outbox;
    }

    public async Task<Result<object>> Handle(CreateAssetCategoryCommand request, CancellationToken cancellationToken)
    {
        // Generate unique code for asset category
        var code = await _unitOfWork.GenerateNextCodeAsync("NTS", "AssetCategories", 6, cancellationToken);
        
        var assetCategory = new AssetCategory(
            code: code, 
            name: request.Name,
            description: request.Description,
            isActive: request.IsActive
        );

        await _repository.AddAsync(assetCategory);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = assetCategory.Id,
            Code = assetCategory.Code
        });
    }
}
