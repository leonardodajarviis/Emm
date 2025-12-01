using Emm.Application.Features.AppParameterCatalog.Dtos;
using Emm.Application.Common;

namespace Emm.Application.Features.AppParameterCatalog.Queries;

public record GetParameterCatalogByIdQuery(long Id) : IRequest<Result<ParameterCatalogResponse>>;
