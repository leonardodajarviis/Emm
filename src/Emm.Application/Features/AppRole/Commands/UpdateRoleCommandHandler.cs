using Emm.Domain.Repositories;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Commands;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleRepository _repository;

    public UpdateRoleCommandHandler(IUnitOfWork unitOfWork, IRoleRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (role == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Role not found.");
        }

        role.Update(request.Name, request.Description);
        _repository.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = role.Id,
            Code = role.Code
        });
    }
}
