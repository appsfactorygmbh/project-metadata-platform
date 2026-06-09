using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.OfficeLocations.Models;
using ProjectMetadataPlatform.Application.OfficeLocations;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Api.OfficeLocations;

/// <summary>
/// Endpoints for managing Office Locations.
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
[Route("[controller]")]
public class OfficeLocationsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="OfficeLocationsController" />.
    /// </summary>
    /// <param name="mediator"></param>
    public OfficeLocationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all Office Locations.
    /// </summary>
    /// <returns>List of Office Locations</returns>
    /// <response code="200">TheOffice Locations are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetOfficeLocationResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetOfficeLocationResponse>>> Get()
    {
        var query = new GetAllOfficeLocationsQuery();
        var locations = await _mediator.Send(query);
        var response = locations.Select(location => new GetOfficeLocationResponse(
            Id: location.Id,
            OfficeLocationName: location.OfficeLocationName
        ));

        return Ok(response);
    }

    /// <summary>
    /// Returns a Office Location specified by id.
    /// </summary>
    /// <param name="id">Id of the Office Location.</param>
    /// <returns>The specified Office Location.</returns>
    /// <response code="200"> Office Location returned succesfully.</response>
    /// <response code="404"> Office Location not found. </response>
    /// <response code="500"> internal error. </response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetOfficeLocationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetOfficeLocationResponse>>> Get(int id)
    {
        var query = new GetOfficeLocationQuery(id);
        var location = await _mediator.Send(query);
        var response = new GetOfficeLocationResponse(
            Id: location.Id,
            OfficeLocationName: location.OfficeLocationName
        );

        return Ok(response);
    }

    /// <summary>
    /// Creates a new Office Location.
    /// </summary>
    /// <param name="request">Request to create a Office Location.</param>
    /// <returns>Id of the newly created Office Location.</returns>
    /// <response code="201"> Office Location was created succesfuly.</response>
    /// <response code="400">Office Location couldn't be created. </response>
    /// <response code="409">Office Location with same Name already exists. </response>
    /// <response code="500"> Internal error. </response>
    [HttpPut]
    [ProducesResponseType(typeof(CreateOfficeLocationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateOfficeLocationResponse>> Put(
        [FromBody] CreateOfficeLocationRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.OfficeLocationName))
        {
            return BadRequest(
                new ErrorResponse("Office Location Name can't be empty or whitespaces")
            );
        }
        var command = new CreateOfficeLocationCommand(
            OfficeLocationName: request.OfficeLocationName
        );

        var locationId = await _mediator.Send(command);

        var response = new CreateOfficeLocationResponse(locationId);
        var uri = "OfficeLocations/" + locationId;
        return Created(uri, response);
    }

    /// <summary>
    /// Updates a specified Office Location.
    /// </summary>
    /// <param name="id">If of the Office Location.</param>
    /// <param name="request">Update Request.</param>
    /// <returns>The updated Office Location.</returns>
    /// <response code="200"> Office Location was updated successfully. </response>
    /// <response code="400"> Office Location could not be updated. </response>
    /// <response code="404"> Office Location couldn't be found. </response>
    /// <response code="409"> New Office Location name already exists. </response>
    /// <response code="500"> Internal error. </response>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(GetOfficeLocationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetOfficeLocationResponse>> Patch(
        int id,
        [FromBody] UpdateOfficeLocationRequest request
    )
    {
        if (
            request.OfficeLocationName != null
            && string.IsNullOrWhiteSpace(request.OfficeLocationName)
        )
        {
            return BadRequest(new ErrorResponse("Office Location Name can't be whitespaces"));
        }
        var command = new UpdateOfficeLocationCommand(
            Id: id,
            OfficeLocationName: request.OfficeLocationName
        );
        var location = await _mediator.Send(command);

        var response = new GetOfficeLocationResponse(
            Id: location.Id,
            OfficeLocationName: location.OfficeLocationName
        );

        return Ok(response);
    }

    /// <summary>
    /// Deletes a Office Location specified by Id.
    /// </summary>
    /// <param name="id">Id of the Office Location to be deleted.</param>
    /// <returns>No Content</returns>
    /// <response code="204">The Office Location was deleted successfully.</response>
    /// <response code="404"> The Office Location couldn't be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteOfficeLocationCommand(id);

        await _mediator.Send(command);

        return NoContent();
    }
}
