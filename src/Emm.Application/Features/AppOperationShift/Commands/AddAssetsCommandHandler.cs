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
            return Result.NotFound("Không tìm thấy ca vận hành");
        }

        if (data.AssetIds.Count == 0)
        {
            return Result.Validation("Không có tài sản để thêm");
        }

        var assetDict = await _qq.Query<Asset>()
            .Include(a => a.Parameters)
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
        foreach (var parameters in assetDict.Values.Select(a => a.Parameters))
        {
            foreach (var parameter in parameters)
            {
                shift.AddReadingSnapshot(
                    parameter.AssetId,
                    parameter.ParameterId,
                    parameter.CurrentValue);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
