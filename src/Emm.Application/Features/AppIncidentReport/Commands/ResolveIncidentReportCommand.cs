using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppIncidentReport.Commands;

public record ResolveIncidentReportCommand(
    Guid Id,
    string ResolutionNotes
) : IRequest<Result<object>>;
