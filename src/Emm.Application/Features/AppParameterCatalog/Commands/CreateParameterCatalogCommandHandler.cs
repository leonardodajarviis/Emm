using Emm.Application.Common;
using Emm.Domain.Entities;

namespace Emm.Application.Features.AppParameterCatalog.Commands;

public class CreateParameterCatalogCommandHandler : IRequestHandler<CreateParameterCatalogCommand, Result<object>>
{
    private readonly IRepository<ParameterCatalog, long> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateParameterCatalogCommandHandler(
        IRepository<ParameterCatalog, long> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<object>> Handle(CreateParameterCatalogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Generate unique code for parameter catalog
            var code = await _unitOfWork.GenerateNextCodeAsync("TS", "ParameterCatalogs", 6, cancellationToken);

            var parameterCatalog = new ParameterCatalog(
                code,
                request.Name,
                request.Description);

            await _repository.AddAsync(parameterCatalog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new
            {
                Id = parameterCatalog.Id,
                Code = parameterCatalog.Code
            });
        }
        catch (Exception ex)
        {
            return Result<object>.Failure(ErrorType.Internal, ex.Message);
        }
    }
}
