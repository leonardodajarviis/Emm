using Emm.Domain.Entities.Authorization;
using Emm.Domain.Repositories;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Commands;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRoleRepository _repository;

    public CreateRoleCommandHandler(IUnitOfWork unitOfWork, IRoleRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        // Check if role code already exists
        var existingRole = await _repository.GetByCodeAsync(request.Code, cancellationToken);
        if (existingRole != null)
        {
            return Result<object>.Failure(ErrorType.Conflict, "Role code already exists.");
        }

        var role = new Role(
            code: request.Code,
            name: request.Name,
            description: request.Description,
            isSystemRole: request.IsSystemRole
        );

        await _repository.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = role.Id,
            Code = role.Code
        });
    }
}
