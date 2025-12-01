using Emm.Application.Abstractions;
using Emm.Domain.Entities;
using Emm.Domain.Repositories;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(
        IUnitOfWork unitOfWork, 
        IUserRepository repository,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<object>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if username already exists
        var existingUser = await _repository.GetByUsernameAsync(request.Username, cancellationToken);
        if (existingUser != null)
        {
            return Result<object>.Failure(ErrorType.Conflict, "Username already exists.");
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User(
            username: request.Username,
            displayName: request.DisplayName,
            passwordHash: passwordHash,
            email: request.Email
        );

        await _repository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = user.Id,
            Username = user.Username
        });
    }
}
