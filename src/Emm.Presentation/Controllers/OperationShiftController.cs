using Emm.Application.Features.AppOperationShift.Commands;
using Emm.Application.Features.AppOperationShift.Dtos;
using Emm.Application.Features.AppOperationShift.Queries;
using Emm.Application.Common;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;

namespace Emm.Presentation.Controllers;

// [Authorize]
[ApiController]
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

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdateOperationShift updateOperationShift)
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

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        var command = new DeleteOperationShiftCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById([FromRoute] long id)
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
    [HttpPost("{id:long}/complete")]
    public async Task<IActionResult> Complete([FromRoute] long id, [FromBody] CompleteShiftRequest request)
    {
        var command = new CompleteShiftCommand
        {
            ShiftId = id,
            ActualEndTime = request.ActualEndTime,
            Notes = request.Notes
        };
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("{id:long}/start")]
    public async Task<IActionResult> Start([FromRoute] long id)
    {
        var command = new StartOperationShiftCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("{id:long}/cancel")]
    public async Task<IActionResult> Cancel([FromRoute] long id, [FromBody] CancelShiftRequest request)
    {
        var command = new CancelShiftCommand
        {
            ShiftId = id,
            Reason = request.Reason
        };
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("{id:long}/shift-logs")]
    public async Task<IActionResult> AddShiftLog([FromRoute] long id, [FromBody] AddShiftLogCommand request)
    {
        request.OperationShiftId = id;
        var result = await _mediator.Send(request);
        return result.ToActionResult();
    }

    [HttpPut("{id:long}/shift-logs/{shiftLogId:long}")]
    public async Task<IActionResult> UpdateShiftLog([FromRoute] long id, [FromRoute] long shiftLogId, [FromBody] UpdateShiftLogCommand request)
    {
        request.OperationShiftId = id;
        request.ShiftLogId = shiftLogId;
        var result = await _mediator.Send(request);
        return result.ToActionResult();
    }

    [HttpDelete("{id:long}/shift-logs/{shiftLogId:long}")]
    public async Task<IActionResult> RemoveShiftLog([FromRoute] long id, [FromRoute] long shiftLogId)
    {
        var command = new RemoveShiftLogCommand
        {
            OperationShiftId = id,
            ShiftLogId = shiftLogId
        };
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPost("{id:long}/assets")]
    public async Task<IActionResult> AddAssets([FromRoute] long id, [FromBody] AddAssetsRequest request)
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
