using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppIncidentReport.Commands;

public record ResolveIncidentReportCommand(
    long Id,
    string ResolutionNotes
) : IRequest<Result<object>>;
