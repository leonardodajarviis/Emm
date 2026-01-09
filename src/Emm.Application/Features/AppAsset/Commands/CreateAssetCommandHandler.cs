namespace Emm.Application.Features.AppAsset.Commands;

public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, Result<object>>
{
    // private readonly IUnitOfWork _unitOfWork;
    // private readonly IRepository<Asset, Guid> _repository;
    // private readonly ICodeGenerator _codeGenerator;

    // public CreateAssetCommandHandler(
    //     IUnitOfWork unitOfWork,
    //     ICodeGenerator codeGenerator,
    //     IRepository<Asset, Guid> repository)
    // {
    //     ArgumentNullException.ThrowIfNull(unitOfWork);
    //     ArgumentNullException.ThrowIfNull(repository);
    //     ArgumentNullException.ThrowIfNull(codeGenerator);

    //     _unitOfWork = unitOfWork;
    //     _repository = repository;
    //     _codeGenerator = codeGenerator;
    // }

    public CreateAssetCommandHandler()
    {
    }

    public async Task<Result<object>> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
