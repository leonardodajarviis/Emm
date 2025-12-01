using Emm.Domain.Entities;
using Emm.Domain.Repositories;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Commands;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _repository;

    public UpdateUserCommandHandler(IUnitOfWork unitOfWork, IUserRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "User not found.");
        }

        // Check if username already exists for another user
        var existingUser = await _repository.GetByUsernameAsync(request.Username, cancellationToken);
        if (existingUser != null && existingUser.Id != request.Id)
        {
            return Result<object>.Failure(ErrorType.Conflict, "Username already exists.");
        }

        user.Update(request.Username, request.Email);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = request.Id
        });
    }
}
