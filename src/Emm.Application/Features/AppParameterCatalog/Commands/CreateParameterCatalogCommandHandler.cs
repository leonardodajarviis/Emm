using Emm.Application.Common;
using Emm.Domain.Entities;

namespace Emm.Application.Features.AppParameterCatalog.Commands;

public class CreateParameterCatalogCommandHandler : IRequestHandler<CreateParameterCatalogCommand, Result<object>>
{
    private readonly IRepository<ParameterCatalog, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateParameterCatalogCommandHandler(
        IRepository<ParameterCatalog, Guid> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<object>> Handle(CreateParameterCatalogCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var code = await _unitOfWork.GenerateNextCodeAsync("TS", "ParameterCatalogs", 6, cancellationToken);

            var parameterCatalog = new ParameterCatalog(
                code,
                request.Name,
                request.UnitOfMeasureId,
                request.Description);

            await _repository.AddAsync(parameterCatalog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new
            {
                parameterCatalog.Id,
            });

        });
        // Generate unique code for parameter catalog
    }
}
