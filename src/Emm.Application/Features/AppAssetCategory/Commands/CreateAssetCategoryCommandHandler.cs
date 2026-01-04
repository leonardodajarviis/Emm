using Emm.Application.Abstractions;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.ValueObjects;

namespace Emm.Application.Features.AppAssetCategory.Commands;

public class CreateAssetCategoryCommandHandler : IRequestHandler<CreateAssetCategoryCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<AssetCategory, Guid> _repository;
    private readonly IUserContextService _userContextService;
    private readonly ICodeGenerator _codeGenerator;

    public CreateAssetCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<AssetCategory, Guid> repository,
        ICodeGenerator codeGenerator,
        IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _userContextService = userContextService;
        _codeGenerator = codeGenerator;
    }

    public async Task<Result<object>> Handle(CreateAssetCategoryCommand request, CancellationToken cancellationToken)
    {
        // Generate unique code for asset category
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            NaturalKey code = new();

            if (request.IsCodeGenerated)
            {
                code = await _codeGenerator.GetNaturalKeyAsync<AssetCategory>("NTB", 6, cancellationToken);
            }
            else
            {
                code = NaturalKey.CreateRaw(request.Code);
            }

            var assetCategory = new AssetCategory(
                code: code,
                isCodeGenerated: request.IsCodeGenerated,
                name: request.Name,
                description: request.Description,
                isActive: request.IsActive
            );

            var currentUserid = _userContextService.GetCurrentUserId();

            await _repository.AddAsync(assetCategory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new
            {
                assetCategory.Id,
            });

        });
    }
}
