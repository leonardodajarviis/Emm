using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppIncidentReport.Commands;

public record AssignIncidentReportCommand(long Id) : IRequest<Result<object>>;

public record StartIncidentReportProgressCommand(long Id) : IRequest<Result<object>>;

public record CloseIncidentReportCommand(long Id) : IRequest<Result<object>>;
