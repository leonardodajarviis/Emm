using Emm.Domain.Entities.Maintenance;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppIncidentReport.Commands;

public record UpdateIncidentReportCommand(
    Guid Id,
    string Title,
    string Description,
    IncidentPriority Priority
) : IRequest<Result<object>>;
