namespace Emm.Application.Features.AppAssetAddition.Queries;

public record GetAssetAdditionByIdQuery(Guid Id) : IRequest<Result<object>>;
