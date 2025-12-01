using Emm.Application.Abstractions;
using Emm.Application.Common.ErrorCodes;
using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class AddAssetsCommandHandler : IRequestHandler<AddAssetsCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOperationShiftRepository _repository;
    private readonly IQueryContext _qq;

    public AddAssetsCommandHandler(
        IUnitOfWork unitOfWork,
        IOperationShiftRepository repository,
        IQueryContext queryContext)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _qq = queryContext;
    }

    public async Task<Result<object>> Handle(AddAssetsCommand request, CancellationToken cancellationToken)
    {
        var shift = await _repository.GetByIdAsync(request.ShiftId, cancellationToken);
        if (shift == null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        if (request.AssetIds.Count == 0)
        {
            return Result<object>.Validation("No assets provided", ValidationErrorCodes.FieldRequired);
        }

        var assetDict = await _qq.Query<Asset>()
            .Where(a => request.AssetIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);

        foreach (var assetId in request.AssetIds)
        {
            if (assetDict.TryGetValue(assetId, out var existingAsset))
            {
                shift.AddAsset(
                    assetId: existingAsset.Id,
                    assetCode: existingAsset.Code,
                    assetName: existingAsset.DisplayName);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success("Assets added successfully");
    }
}
