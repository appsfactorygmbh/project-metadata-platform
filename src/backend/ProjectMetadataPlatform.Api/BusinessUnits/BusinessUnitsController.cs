using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.BusinessUnits.Models;
using ProjectMetadataPlatform.Api.Common.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Application.BusinessUnits;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Api.BusinessUnits;

/// <summary>
/// Endpoints for managing Business Units.
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
[Route("[controller]")]
public class BusinessUnitsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="BusinessUnitsController" />.
    /// </summary>
    /// <param name="mediator"></param>
    public BusinessUnitsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all Business Units.
    /// </summary>
    /// <returns>List of Business Units with permissions on the resource type.</returns>
    /// <response code="200">The business Units are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet]
    [ProducesResponseType(
        typeof(GetListResponse<GetBusinessUnitResponse>),
        StatusCodes.Status200OK
    )]
    public async Task<ActionResult<GetListResponse<GetBusinessUnitResponse>>> Get()
    {
        var query = new GetAllBusinessUnitsQuery();
        var (businessUnits, permissions) = await _mediator.Send(query);
        var buResponse = businessUnits.Select(businessUnit => new GetBusinessUnitResponse(
            Id: businessUnit.Id,
            BusinessUnitName: businessUnit.BusinessUnitName
        ));
        var response = new GetListResponse<GetBusinessUnitResponse>(
            [.. buResponse],
            [.. permissions]
        );
        return Ok(response);
    }

    /// <summary>
    /// Returns a bu specified by id.
    /// </summary>
    /// <param name="id">Id of the bu.</param>
    /// <returns>The specified bu with allowed actions for it.</returns>
    /// <response code="200"> bu returned succesfully.</response>
    /// <response code="404"> bu not found. </response>
    /// <response code="500"> internal error. </response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetBusinessUnitResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetBusinessUnitResponse>>> Get(int id)
    {
        var query = new GetBusinessUnitQuery(id);
        var (businessUnit, permissions) = await _mediator.Send(query);
        var response = new GetBusinessUnitResponse(
            Id: businessUnit.Id,
            BusinessUnitName: businessUnit.BusinessUnitName,
            [.. permissions]
        );

        return Ok(response);
    }

    /// <summary>
    /// Creates a new Business Unit.
    /// </summary>
    /// <param name="request">Request to create a Business Unit.</param>
    /// <returns>Id of the newly created bu.</returns>
    /// <response code="201"> Bu was created succesfuly.</response>
    /// <response code="400">Bu couldn't be created. </response>
    /// <response code="409">Bu with same Name already exists. </response>
    /// <response code="500"> Internal error. </response>
    [HttpPut]
    [ProducesResponseType(typeof(CreateBusinessUnitResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateBusinessUnitResponse>> Put(
        [FromBody] CreateBusinessUnitRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.BusinessUnitName))
        {
            return BadRequest(new ErrorResponse("BusinessUnit Name can't be empty or whitespaces"));
        }
        var command = new CreateBusinessUnitCommand(BusinessUnitName: request.BusinessUnitName);

        var businessUnitId = await _mediator.Send(command);

        var response = new CreateBusinessUnitResponse(businessUnitId);
        var uri = "BusinessUnits/" + businessUnitId;
        return Created(uri, response);
    }

    /// <summary>
    /// Updates a specified Business Unit.
    /// </summary>
    /// <param name="id">If of the Bu.</param>
    /// <param name="request">Update Request.</param>
    /// <returns>The updated Business Unit.</returns>
    /// <response code="200"> Business Unit was updated successfully. </response>
    /// <response code="400"> Business Unit could not be updated. </response>
    /// <response code="404"> Bu couldn't be found. </response>
    /// <response code="409"> New Bu name already exists. </response>
    /// <response code="500"> Internal error. </response>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(GetBusinessUnitResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetBusinessUnitResponse>> Patch(
        int id,
        [FromBody] UpdateBusinessUnitRequest request
    )
    {
        if (request.BusinessUnitName != null && string.IsNullOrWhiteSpace(request.BusinessUnitName))
        {
            return BadRequest(new ErrorResponse("Business Unit Name can't be whitespaces"));
        }
        var command = new UpdateBusinessUnitCommand(
            Id: id,
            BusinessUnitName: request.BusinessUnitName
        );
        var businessUnit = await _mediator.Send(command);

        var response = new GetBusinessUnitResponse(
            Id: businessUnit.Id,
            BusinessUnitName: businessUnit.BusinessUnitName
        );

        return Ok(response);
    }

    /// <summary>
    /// Deletes a Business Unit specified by Id.
    /// </summary>
    /// <param name="id">Id of the BU to be deleted.</param>
    /// <returns>No Content</returns>
    /// <response code="204">The bu was deleted successfully.</response>
    /// <response code="400"> The bu couldn't be deleted. </response>
    /// <response code="404"> The bu couldn't be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteBusinessUnitCommand(id);

        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Returns the id's of all Teams linked to the specified bu.
    /// </summary>
    /// <param name="id">Id of the bu.</param>
    /// <returns>List of team Id's</returns>
    /// <response code="200"> Id's returned succesfully </response>
    /// <response code="404"> Business Unit could not be found. </response>
    /// <response code="500"> Internal error. </response>
    [HttpGet("{id:int}/linkedTeams")]
    [ProducesResponseType(typeof(GetLinkedTeamsForBusinessUnitResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetLinkedTeamsForBusinessUnitResponse>> GetLinkedTeams(int id)
    {
        var command = new GetLinkedTeamsQuery(id);

        var idList = await _mediator.Send(command);

        return Ok(new GetLinkedTeamsForBusinessUnitResponse(idList));
    }
}
