using Emm.Application.Features.AppIncidentReport.Commands;
using Emm.Application.Features.AppIncidentReport.Queries;
using Emm.Application.Common;
using Emm.Domain.Entities.Maintenance;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/incident-reports")]
public class IncidentReportController : ControllerBase
{
    private readonly IMediator _mediator;

    public IncidentReportController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateIncidentReport([FromBody] CreateIncidentReport request)
    {
        var command = new CreateIncidentReportCommand(
            Title: request.Title,
            Description: request.Description,
            AssetId: request.AssetId,
            Priority: request.Priority
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateIncidentReport([FromRoute] Guid id, [FromBody] UpdateIncidentReport request)
    {
        var command = new UpdateIncidentReportCommand(
            Id: id,
            Title: request.Title,
            Description: request.Description,
            Priority: request.Priority
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetIncidentReports([FromQuery] QueryParam queryRequest)
    {
        var query = new GetIncidentReportsQuery(queryRequest);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetIncidentReportById([FromRoute] Guid id)
    {
        var query = new GetIncidentReportByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/resolve")]
    public async Task<IActionResult> ResolveIncidentReport([FromRoute] Guid id, [FromBody] ResolveIncidentReport request)
    {
        var command = new ResolveIncidentReportCommand(
            Id: id,
            ResolutionNotes: request.ResolutionNotes
        );

        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPost("{id:long}/assign")]
    public async Task<IActionResult> AssignIncidentReport([FromRoute] long id)
    {
        var command = new AssignIncidentReportCommand(id);
        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPost("{id:long}/start-progress")]
    public async Task<IActionResult> StartIncidentReportProgress([FromRoute] long id)
    {
        var command = new StartIncidentReportProgressCommand(id);
        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPost("{id:long}/close")]
    public async Task<IActionResult> CloseIncidentReport([FromRoute] long id)
    {
        var command = new CloseIncidentReportCommand(id);
        var result = await _mediator.Send(command);

        return result.ToActionResult();
    }
}

public record CreateIncidentReport(
    string Title,
    string Description,
    Guid AssetId,
    IncidentPriority Priority
);

public record UpdateIncidentReport(
    string Title,
    string Description,
    IncidentPriority Priority
);

public record ResolveIncidentReport(
    string ResolutionNotes
);
