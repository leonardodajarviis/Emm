using Emm.Application.Abstractions;
using Emm.Application.Common.ErrorCodes;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class CreateOperationShiftCommandHandler : IRequestHandler<CreateOperationShiftCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOperationShiftRepository _repository;
    private readonly IQueryContext _qq;
    private readonly IUserContextService _userContextService;
    private readonly IOutbox _outbox;
    private readonly ICodeGenerator _codeGenerator;

    public CreateOperationShiftCommandHandler(
        IUnitOfWork unitOfWork,
        IOperationShiftRepository repository,
        IQueryContext queryContext,
        ICodeGenerator codeGenerator,
        IUserContextService userContextService,

        IOutbox outbox)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _qq = queryContext;
        _userContextService = userContextService;
        _outbox = outbox;
        _codeGenerator = codeGenerator;
    }

    public async Task<Result<object>> Handle(CreateOperationShiftCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var code = await _codeGenerator.GenerateNextCodeAsync("NKVH", "OperationShifts", 6, cancellationToken);

            var organizationUnitId = _userContextService.GetCurrentOrganizationUnitId();
            if (organizationUnitId == null)
            {
                return Result<object>.Validation(
                    "Organization unit is required",
                    ValidationErrorCodes.FieldRequired);
            }

            var userId = _userContextService.GetCurrentUserId();
            if (userId == null)
            {
                return Result<object>.Unauthorized(
                    "User information not found",
                    AuthErrorCodes.SessionInvalid);
            }

            var operationShift = new OperationShift(
                code: code,
                name: request.Name,
                primaryEmployeeId: userId.Value,
                organizationUnitId: organizationUnitId.Value,
                scheduledStartTime: DateTime.UtcNow,
                scheduledEndTime: DateTime.UtcNow.AddHours(12),
                notes: request.Notes
            );

            var assets = await _qq.Query<Asset>()
                .Where(a => request.AssetIds.Contains(a.Id))
                .ToListAsync(cancellationToken);

            // Add assets if provided
            foreach (var asset in assets)
            {
                operationShift.AddAsset(
                    assetId: asset.Id,
                    assetCode: asset.Code,
                    assetName: asset.DisplayName
                );
            }
            operationShift.StartShift(DateTime.UtcNow);
            await _repository.AddAsync(operationShift, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new
            {
                operationShift.Id,
                operationShift.Code
            });
        });
    }
}
