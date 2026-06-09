using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Departments.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Application.Departments;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Api.Departments;

/// <summary>
/// Endpoints for managing Departments.
/// </summary>
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

    /// <summary>
    /// Gets all Departments.
    /// </summary>
    /// <returns>List of Departments. </returns>
    /// <response code="200">The Departments are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
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

    /// <summary>
    /// Returns a department specified by id.
    /// </summary>
    /// <param name="id">Id of the department.</param>
    /// <returns>The specified department.</returns>
    /// <response code="200"> department returned succesfully.</response>
    /// <response code="404"> department not found. </response>
    /// <response code="500"> internal error. </response>
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

    /// <summary>
    /// Creates a new department.
    /// </summary>
    /// <param name="request">Request to create a department.</param>
    /// <returns>Id of the newly created department.</returns>
    /// <response code="201"> department was created succesfuly.</response>
    /// <response code="400"> department couldn't be created. </response>
    /// <response code="409"> department with same Name already exists. </response>
    /// <response code="500"> Internal error. </response>
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

        var response = new CreateDepartmentResponse(departmentId);
        var uri = "Departments/" + departmentId;
        return Created(uri, response);
    }

    /// <summary>
    /// Updates a specified department.
    /// </summary>
    /// <param name="id">If of the department.</param>
    /// <param name="request">Update Request.</param>
    /// <returns>The updated department.</returns>
    /// <response code="200"> department was updated successfully. </response>
    /// <response code="400"> department could not be updated. </response>
    /// <response code="404"> department couldn't be found. </response>
    /// <response code="409"> New department name already exists. </response>
    /// <response code="500"> Internal error. </response>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(GetDepartmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetDepartmentResponse>> Patch(
        int id,
        [FromBody] UpdateDepartmentRequest request
    )
    {
        if (request.DepartmentName != null && string.IsNullOrWhiteSpace(request.DepartmentName))
        {
            return BadRequest(new ErrorResponse("Department Name can't be whitespaces"));
        }
        var command = new UpdateDepartmentCommand(Id: id, DepartmentName: request.DepartmentName);
        var department = await _mediator.Send(command);

        var response = new GetDepartmentResponse(
            Id: department.Id,
            DepartmentName: department.DepartmentName
        );

        return Ok(response);
    }

    /// <summary>
    /// Deletes a department specified by Id.
    /// </summary>
    /// <param name="id">Id of the department to be deleted.</param>
    /// <returns>No Content</returns>
    /// <response code="204">The department was deleted successfully.</response>
    /// <response code="404"> The department couldn't be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteDepartmentCommand(id);

        await _mediator.Send(command);

        return NoContent();
    }
}
