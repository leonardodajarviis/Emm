using Emm.Domain.Abstractions;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.ValueObjects;

namespace Emm.Application.Features.AppAsset.Commands;

public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Asset, Guid> _repository;
    private readonly ICodeGenerator _codeGenerator;

    public CreateAssetCommandHandler(
        IUnitOfWork unitOfWork,
        ICodeGenerator codeGenerator,
        IRepository<Asset, Guid> repository)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(codeGenerator);

        _unitOfWork = unitOfWork;
        _repository = repository;
        _codeGenerator = codeGenerator;
    }

    public async Task<Result<object>> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        NaturalKey code = new();

        if (request.IsCodeGenerated)
        {
            code = await _codeGenerator.GetNaturalKeyAsync<Asset>("TB", 10, cancellationToken);
        }
        else
        {
            if (request.Code == null)
            {
                throw new ArgumentNullException(nameof(request.Code), "Code cannot be null when IsCodeGenerated is false.");
            }
            code = NaturalKey.CreateRaw(request.Code);
        }

        var asset = new Asset(
            code: code,
            displayName: request.DisplayName,
            assetModelId: request.AssetModelId,
            assetCategoryId: null, // TODO: get from AssetModel
            assetTypeId: null, // TODO: get from AssetModel
            organizationUnitId: Guid.Empty, // TODO: get from user context
            locationId: Guid.Empty, // TODO: get from user context
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
