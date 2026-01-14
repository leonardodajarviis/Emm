using Emm.Application.Common;
using Emm.Domain.Entities;

namespace Emm.Application.Features.AppUnitOfMeasure.Commands;

public class CreateUnitOfMeasureCommandHandler : IRequestHandler<CreateUnitOfMeasureCommand, Result<object>>
{
    private readonly IRepository<UnitOfMeasure, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUnitOfMeasureCommandHandler(
        IRepository<UnitOfMeasure, Guid> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<object>> Handle(CreateUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate base unit if specified
            if (request.BaseUnitId.HasValue)
            {
                var baseUnit = await _repository.GetByIdAsync(request.BaseUnitId.Value, cancellationToken);
                if (baseUnit == null)
                {
                    return Result<object>.Failure(ErrorType.NotFound, "Base unit not found");
                }
            }

            // Generate unique code for unit of measure
            var code = await _unitOfWork.GenerateNextCodeAsync<UnitOfMeasure>("UOM", 6, cancellationToken);

            var unitOfMeasure = new UnitOfMeasure(
                code,
                request.Name,
                request.Symbol,
                request.Description,
                request.BaseUnitId,
                request.ConversionFactor);

            await _repository.AddAsync(unitOfMeasure, cancellationToken);
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
