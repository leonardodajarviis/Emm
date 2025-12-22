using Emm.Application.Abstractions;
using Emm.Domain.Entities.AssetTransaction;

namespace Emm.Application.Features.AppAssetAddition.Commands;

public class CreateAssetAdditionCommandHandler : IRequestHandler<CreateAssetAdditionCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<AssetAddition, Guid> _repository;
    private readonly IOutbox _outbox;

    public CreateAssetAdditionCommandHandler(IUnitOfWork unitOfWork, IRepository<AssetAddition, Guid> repository, IOutbox outbox)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _outbox = outbox;
    }

    public async Task<Result<object>> Handle(CreateAssetAdditionCommand request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // Generate the next code for Asset Addition
            var code = await _unitOfWork.GenerateNextCodeAsync<AssetAddition>("PNTS", 6, cancellationToken);

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
                assetAddition.AddAssetAdditionLine(
                    assetModelId: lineCommand.AssetModelId,
                    assetCode: lineCommand.AssetCode,
                    unitPrice: lineCommand.UnitPrice
                );
            }

            await _repository.AddAsync(assetAddition);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Register event to outbox - will be processed after transaction commits
            assetAddition.RegisterEvent();
            _outbox.EnqueueRange(assetAddition.DomainEvents);
            assetAddition.ClearDomainEvents();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new
            {
                assetAddition.Id
            });
        });

        return result;
    }
}
