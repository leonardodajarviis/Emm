using Emm.Application.Features.AppOperationShift.Commands;
using Emm.Application.Features.AppOperationShift.Dtos;
using Emm.Application.Features.AppOperationShift.Queries;
using Emm.Application.Common;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateOperationShift updateOperationShift)
    {
        var command = new UpdateOperationShiftCommand(
            Id: id,
            Name: updateOperationShift.Name,
            Description: updateOperationShift.Description,
            LocationId: updateOperationShift.LocationId,

            ScheduledStartTime: updateOperationShift.ScheduledStartTime,
            ScheduledEndTime: updateOperationShift.ScheduledEndTime,
            Notes: updateOperationShift.Notes
        );

        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteOperationShiftCommand(id);
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

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, [FromBody] CancelShiftRequest request)
    {
        var command = new CancelShiftCommand
        {
            ShiftId = id,
            Reason = request.Reason
        };
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("{id}/shift-logs")]
    public async Task<IActionResult> AddShiftLog([FromRoute] Guid id, [FromBody] AddShiftLogCommand request)
    {
        request.OperationShiftId = id;
        var result = await _mediator.Send(request);
        return result.ToActionResult();
    }

    [HttpPut("{id}/shift-logs/{shiftLogId}")]
    public async Task<IActionResult> UpdateShiftLog([FromRoute] Guid id, [FromRoute] Guid shiftLogId, [FromBody] UpdateShiftLogCommand request)
    {
        request.OperationShiftId = id;
        request.ShiftLogId = shiftLogId;
        var result = await _mediator.Send(request);
        return result.ToActionResult();
    }

    [HttpDelete("{id}/shift-logs/{shiftLogId}")]
    public async Task<IActionResult> RemoveShiftLog([FromRoute] Guid id, [FromRoute] Guid shiftLogId)
    {
        var command = new RemoveShiftLogCommand
        {
            OperationShiftId = id,
            ShiftLogId = shiftLogId
        };
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("{id}/assets")]
    public async Task<IActionResult> AddAssets([FromRoute] Guid id, [FromBody] AddAssetsRequest request)
    {
        var command = new AddAssetsCommand
        {
            ShiftId = id,
            AssetIds = request.AssetIds
        };
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }
}
