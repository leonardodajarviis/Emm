using Emm.Application.Abstractions;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAssetType.Commands;

public class CreateAssetTypeCommandHandler : IRequestHandler<CreateAssetTypeCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetTypeRepository _repository;
    private readonly IUserContextService _userContextService;
    private readonly ICodeGenerator _codeGenerator;

    public CreateAssetTypeCommandHandler(
        IUnitOfWork unitOfWork,
        IAssetTypeRepository repository,
        ICodeGenerator codeGenerator,
        IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _userContextService = userContextService;
        _codeGenerator = codeGenerator;
    }

    public async Task<Result<object>> Handle(CreateAssetTypeCommand request, CancellationToken cancellationToken)
    {
        NaturalKey code = new();

        if (request.IsCodeGenerated)
        {
            code = await _codeGenerator.GetNaturalKeyAsync<AssetType>("DTB", 6, cancellationToken);
        }
        else
        {
            code = NaturalKey.CreateRaw(request.Code);
        }

        var assetType = new AssetType(
            request.IsCodeGenerated,
            code: code,
            name: request.Name,
            assetCategoryId: request.AssetCategoryId,
            description: request.Description,
            isActive: request.IsActive
        );

        var currentUserId = _userContextService.GetCurrentUserId();
        assetType.AddParameters(request.ParameterIds);

        await _repository.AddAsync(assetType);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            assetType.Id
        });

    }
}
