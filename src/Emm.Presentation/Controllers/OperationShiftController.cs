using Emm.Application.Features.AppOperationShift.Commands;
using Emm.Application.Features.AppOperationShift.Queries;
using Emm.Application.Common;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Emm.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/operation-shifts")]
public class OperationShiftController : ControllerBase
{
    private readonly IMediator _mediator;

    public OperationShiftController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOperationShiftCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var query = new GetOperationShiftByIdQuery(id);
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> Gets([FromQuery] QueryParam QueryRequest)
    {
        var query = new GetOperationShiftsQuery(QueryRequest);
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    // Business operations endpoints
    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Complete([FromRoute] Guid id)
    {
        var command = new CompleteShiftCommand
        {
            ShiftId = id,
        };
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> Start([FromRoute] Guid id)
    {
        var command = new StartOperationShiftCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }


    [HttpPost("{id}/shift-logs")]
    public async Task<IActionResult> AddShiftLog([FromRoute] Guid id, [FromBody] CreateShiftLogData data)
    {
        var command = new CreateShiftLogCommand
        {
            OperationShiftId = id,
            Data = data
        };
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("{id}/shift-logs/{shiftLogId}")]
    public async Task<IActionResult> UpdateShiftLog([FromRoute] Guid id, [FromRoute] Guid shiftLogId, [FromBody] UpdateShiftLogData data)
    {
        var command = new UpdateShiftLogCommand
        {
            OperationShiftId = id,
            ShiftLogId = shiftLogId,
            Data = data
        };
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("{id}/assets")]
    public async Task<IActionResult> AddAssets([FromRoute] Guid id, [FromBody] AddAssetsData data)
    {
        var command = new AddAssetsCommand
        {
            ShiftId = id,
            Data = data
        };
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }
}
