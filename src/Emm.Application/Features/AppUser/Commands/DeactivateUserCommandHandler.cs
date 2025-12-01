using Emm.Domain.Entities;
using Emm.Domain.Repositories;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Commands;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _repository;

    public DeactivateUserCommandHandler(IUnitOfWork unitOfWork, IUserRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "User not found.");
        }

        user.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = request.Id,
            IsActive = user.IsActive
        });
    }
}
