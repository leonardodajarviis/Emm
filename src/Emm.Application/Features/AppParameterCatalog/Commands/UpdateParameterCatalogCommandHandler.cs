using Emm.Application.Common;
using Emm.Domain.Entities;

namespace Emm.Application.Features.AppParameterCatalog.Commands;

public class UpdateParameterCatalogCommandHandler : IRequestHandler<UpdateParameterCatalogCommand, Result<bool>>
{
    private readonly IRepository<ParameterCatalog, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateParameterCatalogCommandHandler(
        IRepository<ParameterCatalog, Guid> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateParameterCatalogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var parameterCatalog = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (parameterCatalog is null)
            {
                return Result<bool>.Failure(ErrorType.NotFound, "ParameterCatalog not found");
            }

            parameterCatalog.Update(
                request.Name,
                request.Description);

            _repository.Update(parameterCatalog);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ErrorType.Internal, ex.Message);
        }
    }
}
