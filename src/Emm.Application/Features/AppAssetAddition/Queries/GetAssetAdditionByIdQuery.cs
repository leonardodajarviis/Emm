namespace Emm.Application.Features.AppAssetAddition.Queries;

public record GetAssetAdditionByIdQuery(long Id) : IRequest<Result<object>>;
