using Emm.Domain.Repositories;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Commands;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleRepository _repository;

    public DeleteRoleCommandHandler(IUnitOfWork unitOfWork, IRoleRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (role == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Role not found.");
        }

        if (role.IsSystemRole)
        {
            return Result<object>.Failure(ErrorType.Validation, "Cannot delete system role.");
        }

        _repository.Delete(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = request.Id
        });
    }
}
