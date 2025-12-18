using Emm.Application.Common;
using Emm.Domain.Entities;

namespace Emm.Application.Features.AppUnitOfMeasure.Commands;

public class DeactivateUnitOfMeasureCommandHandler : IRequestHandler<DeactivateUnitOfMeasureCommand, Result<object>>
{
    private readonly IRepository<UnitOfMeasure, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateUnitOfMeasureCommandHandler(
        IRepository<UnitOfMeasure, Guid> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<object>> Handle(DeactivateUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var unitOfMeasure = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (unitOfMeasure == null)
            {
                return Result<object>.Failure(ErrorType.NotFound, "Unit of measure not found");
            }

            unitOfMeasure.Deactivate();
            _repository.Update(unitOfMeasure);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new { Id = request.Id });
        }
        catch (Exception ex)
        {
            return Result<object>.Failure(ErrorType.Internal, ex.Message);
        }
    }
}
