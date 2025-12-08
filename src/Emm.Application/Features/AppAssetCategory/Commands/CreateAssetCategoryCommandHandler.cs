using Emm.Application.Abstractions;
using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetCategory.Commands;

public class CreateAssetCategoryCommandHandler : IRequestHandler<CreateAssetCategoryCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<AssetCategory, long> _repository;
    private readonly IUserContextService _userContextService;

    public CreateAssetCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<AssetCategory, long> repository,
        IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _userContextService = userContextService;
    }

    public async Task<Result<object>> Handle(CreateAssetCategoryCommand request, CancellationToken cancellationToken)
    {
        // Generate unique code for asset category
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var code =request.Code;
            if (request.IsCodeGenerated)
                code = await _unitOfWork.GenerateNextCodeAsync("NTB", "AssetCategories", 6, cancellationToken);

            var assetCategory = new AssetCategory(
                code: code,
                isCodeGenerated: request.IsCodeGenerated,
                name: request.Name,
                description: request.Description,
                isActive: request.IsActive
            );

            var currentUserid = _userContextService.GetCurrentUserId();
            assetCategory.SetAuditInfo(currentUserid, currentUserid);

            await _repository.AddAsync(assetCategory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new
            {
                assetCategory.Id,
            });

        });
    }
}
