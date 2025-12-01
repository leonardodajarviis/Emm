using Emm.Domain.Entities.Maintenance;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppIncidentReport.Commands;

public record UpdateIncidentReportCommand(
    long Id,
    string Title,
    string Description,
    IncidentPriority Priority
) : IRequest<Result<object>>;
