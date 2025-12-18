using Emm.Application.Abstractions;
using Emm.Domain.Entities.Organization;

namespace Emm.Application.Features.AppLocation.Commands;

public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Location, Guid> _repository;

    public UpdateLocationCommandHandler(IUnitOfWork unitOfWork, IRepository<Location, Guid> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _repository.GetByIdAsync(request.Id);
        if (location == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Location not found");
        }

        location.Update(request.Name, request.Description, request.IsActive);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new { Id = location.Id });
    }
}
