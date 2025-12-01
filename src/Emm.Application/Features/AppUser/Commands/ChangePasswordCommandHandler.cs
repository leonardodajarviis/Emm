using Emm.Application.Abstractions;

namespace Emm.Application.Features.AppUser.Commands;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(
        IUnitOfWork unitOfWork, 
        IUserRepository repository,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<object>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "User not found.");
        }

        // Verify current password
        if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            return Result<object>.Failure(ErrorType.Validation, "Current password is incorrect.");
        }

        // Hash new password
        var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.SetPassword(newPasswordHash);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = request.Id,
            Message = "Password changed successfully"
        });
    }
}
