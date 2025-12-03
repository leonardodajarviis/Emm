using Emm.Domain.Repositories;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Commands;

public class DeactivateRoleCommandHandler : IRequestHandler<DeactivateRoleCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleRepository _repository;

    public DeactivateRoleCommandHandler(IUnitOfWork unitOfWork, IRoleRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(DeactivateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (role == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Role not found.");
        }

        if (role.IsSystemRole)
        {
            return Result<object>.Failure(ErrorType.Validation, "Cannot deactivate system role.");
        }

        role.Deactivate();
        _repository.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = role.Id,
            IsActive = role.IsActive
        });
    }
}
