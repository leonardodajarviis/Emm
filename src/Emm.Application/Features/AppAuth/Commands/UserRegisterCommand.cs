using Emm.Application.Abstractions;
using Emm.Application.Features.AppAuth.Dtos;

namespace Emm.Application.Features.AppAuth.Commands;

public record UserRegisterCommand(
    string Username,
    string Password,
    string DisplayName,
    string Email) : IRequest<Result<UserAuthResponse>>, IPublicRequest;
