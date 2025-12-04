using Emm.Application.Abstractions;
using Emm.Application.Features.AppAuth.Dtos;

namespace Emm.Application.Features.AppAuth.Commands;

public record UserLoginCommand(
    string Username,
    string Password,
    string? IpAddress = null,
    string? UserAgent = null) : IRequest<Result<UserAuthResponse>>, IPublicRequest;