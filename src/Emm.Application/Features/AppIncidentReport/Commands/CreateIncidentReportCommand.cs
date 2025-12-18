using Emm.Domain.Entities.Maintenance;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppIncidentReport.Commands;

public record CreateIncidentReportCommand(
    string Title,
    string Description,
    Guid AssetId,
    IncidentPriority Priority
) : IRequest<Result<object>>;
