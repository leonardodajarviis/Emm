using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Commands;

public record ChangePasswordCommand(
    Guid Id,
    string CurrentPassword,
    string NewPassword
) : IRequest<Result<object>>;
