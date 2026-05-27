using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Departments.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Teams.Models;
using ProjectMetadataPlatform.Application.Departments;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Api.Departments;

[ApiController]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
[Route("[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="DepartmentsController" />.
    /// </summary>
    /// <param name="mediator"></param>
    public DepartmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetDepartmentResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetDepartmentResponse>>> Get()
    {
        var query = new GetAllDepartmentsQuery();
        var departments = await _mediator.Send(query);
        var response = departments.Select(department => new GetDepartmentResponse(
            Id: department.Id,
            DepartmentName: department.DepartmentName
        ));

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetDepartmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetDepartmentResponse>>> Get(int id)
    {
        var query = new GetDepartmentQuery(id);
        var department = await _mediator.Send(query);
        var response = new GetDepartmentResponse(
            Id: department.Id,
            DepartmentName: department.DepartmentName
        );

        return Ok(response);
    }

    [HttpPut]
    [ProducesResponseType(typeof(CreateDepartmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateDepartmentResponse>> Put(
        [FromBody] CreateDepartmentRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.DepartmentName))
        {
            return BadRequest(new ErrorResponse("Department Name can't be empty or whitespaces"));
        }
        var command = new CreateDepartmentCommand(DepartmentName: request.DepartmentName);

        var departmentId = await _mediator.Send(command);

        var response = new CreateTeamResponse(departmentId);
        var uri = "Department/" + departmentId;
        return Created(uri, response);
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(GetDepartmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetDepartmentResponse>> Patch(
        int id,
        [FromBody] UpdateDepartmentRequest request
    )
    {
        var command = new UpdateDepartmentCommand(Id: id, DepartmentName: request.DepartmentName);
        var department = await _mediator.Send(command);

        var response = new GetDepartmentResponse(
            Id: department.Id,
            DepartmentName: department.DepartmentName
        );

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(DeleteDepartmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeleteDepartmentResponse>> Delete(int id)
    {
        var command = new DeleteDepartmentCommand(id);

        await _mediator.Send(command);

        return Ok(new DeleteTeamResponse(id));
    }
}
