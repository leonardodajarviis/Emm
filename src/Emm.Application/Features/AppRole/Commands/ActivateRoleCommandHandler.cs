using Emm.Domain.Repositories;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Commands;

public class ActivateRoleCommandHandler : IRequestHandler<ActivateRoleCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleRepository _repository;

    public ActivateRoleCommandHandler(IUnitOfWork unitOfWork, IRoleRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(ActivateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (role == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Role not found.");
        }

        role.Activate();
        _repository.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = role.Id,
            IsActive = role.IsActive
        });
    }
}
