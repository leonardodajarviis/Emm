using Emm.Application.Common;
using Emm.Domain.Entities;

namespace Emm.Application.Features.AppParameterCatalog.Commands;

public class DeleteParameterCatalogCommandHandler : IRequestHandler<DeleteParameterCatalogCommand, Result<bool>>
{
    private readonly IRepository<ParameterCatalog, long> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteParameterCatalogCommandHandler(
        IRepository<ParameterCatalog, long> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteParameterCatalogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var parameterCatalog = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (parameterCatalog is null)
            {
                return Result<bool>.Failure(ErrorType.NotFound, "ParameterCatalog not found");
            }

            _repository.Remove(parameterCatalog);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ErrorType.Internal, ex.Message);
        }
    }
}
