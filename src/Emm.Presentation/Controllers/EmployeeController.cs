using Emm.Application.Features.AppEmployee.Commands;
using Emm.Application.Features.AppEmployee.Dtos;
using Emm.Application.Features.AppEmployee.Queries;
using Emm.Application.Common;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeeController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployee createEmployee)
    {
        var command = new CreateEmployeeCommand(
            DisplayName: createEmployee.DisplayName,
            FirstName: createEmployee.FirstName,
            LastName: createEmployee.LastName,
            OrganizationUnitId: createEmployee.OrganizationUnitId
        );

        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateEmployee([FromRoute] long id, [FromBody] UpdateEmployee updateEmployee)
    {
        var command = new UpdateEmployeeCommand(
            Id: id,
            DisplayName: updateEmployee.DisplayName,
            FirstName: updateEmployee.FirstName,
            LastName: updateEmployee.LastName
        );

        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteEmployee([FromRoute] long id)
    {
        var command = new DeleteEmployeeCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetEmployeeById([FromRoute] long id)
    {
        var query = new GetEmployeeByIdQuery(id);
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees([FromQuery] QueryParam QueryRequest)
    {
        var query = new GetEmployeesQuery(QueryRequest);
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }
}
