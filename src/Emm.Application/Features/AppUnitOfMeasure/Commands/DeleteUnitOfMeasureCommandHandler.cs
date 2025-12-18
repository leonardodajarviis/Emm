using Emm.Application.Common;
using Emm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppUnitOfMeasure.Commands;

public class DeleteUnitOfMeasureCommandHandler : IRequestHandler<DeleteUnitOfMeasureCommand, Result<object>>
{
    private readonly IRepository<UnitOfMeasure, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQueryContext _queryContext;

    public DeleteUnitOfMeasureCommandHandler(
        IRepository<UnitOfMeasure, Guid> repository,
        IUnitOfWork unitOfWork,
        IQueryContext queryContext)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _queryContext = queryContext;
    }

    public async Task<Result<object>> Handle(DeleteUnitOfMeasureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var unitOfMeasure = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (unitOfMeasure == null)
            {
                return Result<object>.Failure(ErrorType.NotFound, "Unit of measure not found");
            }

            // Check if this unit is used as a base unit for other units
            var hasDerivedUnits = await _queryContext.Query<UnitOfMeasure>()
                .AnyAsync(u => u.BaseUnitId == request.Id, cancellationToken);

            if (hasDerivedUnits)
            {
                return Result<object>.Failure(ErrorType.Validation, "Cannot delete unit that is used as a base unit for other units");
            }

            _repository.Remove(unitOfMeasure);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new { Id = request.Id });
        }
        catch (Exception ex)
        {
            return Result<object>.Failure(ErrorType.Internal, ex.Message);
        }
    }
}
