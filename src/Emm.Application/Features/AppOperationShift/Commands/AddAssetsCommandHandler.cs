using Emm.Application.ErrorCodes;
using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class AddAssetsCommandHandler : IRequestHandler<AddAssetsCommand, Result>
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

    public async Task<Result> Handle(AddAssetsCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;
        var shift = await _repository.GetByIdAsync(request.ShiftId, cancellationToken);

        if (shift == null)
        {
            return Result.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        if (data.AssetIds.Count == 0)
        {
            return Result.Validation("No assets provided", ValidationErrorCodes.FieldRequired);
        }

        var assetDict = await _qq.Query<Asset>()
            .Where(a => data.AssetIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);

        foreach (var assetId in data.AssetIds)
        {
            if (assetDict.TryGetValue(assetId, out var existingAsset))
            {
                shift.AddAsset(
                    assetId: existingAsset.Id,
                    assetCode: existingAsset.Code.Value,
                    assetName: existingAsset.DisplayName,
                    isPrimary: false,
                    assetBoxId: data.AssetBoxId);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Assets added successfully");
    }
}
