using Emm.Application.Abstractions;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.AssetTransaction;
using Emm.Domain.ValueObjects;

namespace Emm.Application.Features.AppAssetAddition.Commands;

public class CreateAssetAdditionCommandHandler : IRequestHandler<CreateAssetAdditionCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<AssetAddition, Guid> _repository;
    private readonly IMediator _mediator;
    private readonly ICodeGenerator _codeGenerator;

    public CreateAssetAdditionCommandHandler(IUnitOfWork unitOfWork, IRepository<AssetAddition, Guid> repository, IMediator mediator, ICodeGenerator codeGenerator)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _mediator = mediator;
        _codeGenerator = codeGenerator;
    }

    public async Task<Result<object>> Handle(CreateAssetAdditionCommand request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // Generate the next code for Asset Addition
            var code = await _codeGenerator.GetNaturalKeyAsync<AssetAddition>("PNTB", 10, cancellationToken);

            var assetAddition = new AssetAddition(
                code: code,
                organizationUnitId: request.OrganizationUnitId,
                locationId: request.LocationId,
                decisionNumber: request.DecisionNumber,
                decisionDate: request.DecisionDate,
                reason: request.Reason
            );

            // Add AssetAdditionLines
            foreach (var lineCommand in request.AssetAdditionLines)
            {
                NaturalKey assetCode = new();
                if (lineCommand.IsCodeGenerated)
                {
                    assetCode = await _codeGenerator.GetNaturalKeyAsync<Asset>("TB", 10, cancellationToken);
                }
                if (!lineCommand.IsCodeGenerated)
                {
                    if (string.IsNullOrWhiteSpace(lineCommand.AssetCode))
                        throw new ArgumentException("Asset code cannot be empty", nameof(lineCommand.AssetCode));
                    assetCode = NaturalKey.CreateRaw(lineCommand.AssetCode);
                }
                assetAddition.AddAssetAdditionLine(
                    assetModelId: lineCommand.AssetModelId,
                    isCodeGenerated: lineCommand.IsCodeGenerated,
                    assetCode: assetCode,
                    assetDisplayName: lineCommand.AssetDisplayName,
                    unitPrice: lineCommand.UnitPrice
                );
            }

            assetAddition.RegisterEvent();

            await _repository.AddAsync(assetAddition);

            foreach (var @event in assetAddition.ImmediateEvents)
            {
                await _mediator.Publish(@event, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Register event to outbox - will be processed after transaction commits

            return Result<object>.Success(new
            {
                assetAddition.Id
            });
        });

        return result;
    }
}
