using Emm.Application.Features.AppAuth.Dtos;

namespace Emm.Application.Features.AppAuth.Commands;

public record UserLogoutCommand : IRequest<Result<bool>>;
