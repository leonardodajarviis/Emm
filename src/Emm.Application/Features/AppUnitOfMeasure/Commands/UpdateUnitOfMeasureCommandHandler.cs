using Emm.Application.Common;
using Emm.Domain.Entities;

namespace Emm.Application.Features.AppUnitOfMeasure.Commands;

public class UpdateUnitOfMeasureCommandHandler : IRequestHandler<UpdateUnitOfMeasureCommand, Result<object>>
{
    private readonly IRepository<UnitOfMeasure, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUnitOfMeasureCommandHandler(
        IRepository<UnitOfMeasure, Guid> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<object>> Handle(UpdateUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var unitOfMeasure = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (unitOfMeasure == null)
            {
                return Result<object>.Failure(ErrorType.NotFound, "Unit of measure not found");
            }

            // Validate base unit if specified
            if (request.BaseUnitId.HasValue)
            {
                if (request.BaseUnitId.Value == request.Id)
                {
                    return Result<object>.Failure(ErrorType.Validation, "Unit cannot be its own base unit");
                }

                var baseUnit = await _repository.GetByIdAsync(request.BaseUnitId.Value, cancellationToken);
                if (baseUnit == null)
                {
                    return Result<object>.Failure(ErrorType.NotFound, "Base unit not found");
                }

            }

            unitOfMeasure.Update(
                request.Name,
                request.Symbol,
                request.Description,
                request.BaseUnitId,
                request.ConversionFactor);

            _repository.Update(unitOfMeasure);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new
            {
                Id = unitOfMeasure.Id,
                Code = unitOfMeasure.Code
            });
        }
        catch (Exception ex)
        {
            return Result<object>.Failure(ErrorType.Internal, ex.Message);
        }
    }
}
