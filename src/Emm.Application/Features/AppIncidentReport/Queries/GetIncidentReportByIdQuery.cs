using Emm.Application.Features.AppIncidentReport.Dtos;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppIncidentReport.Queries;

public record GetIncidentReportByIdQuery(Guid Id) : IRequest<Result<IncidentReportResponse>>;
