using Emm.Application.Features.AppAuth.Dtos;

namespace Emm.Application.Features.AppAuth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<UserAuthResponse>>;
