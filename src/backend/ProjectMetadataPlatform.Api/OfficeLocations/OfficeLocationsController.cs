using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.OfficeLocations.Models;
using ProjectMetadataPlatform.Api.Teams.Models;
using ProjectMetadataPlatform.Application.OfficeLocations;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Api.OfficeLocations;

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

        var response = new CreateTeamResponse(locationId);
        var uri = "OfficeLocation/" + locationId;
        return Created(uri, response);
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(GetOfficeLocationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetOfficeLocationResponse>> Patch(
        int id,
        [FromBody] UpdateOfficeLocationRequest request
    )
    {
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

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(DeleteOfficeLocationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeleteOfficeLocationResponse>> Delete(int id)
    {
        var command = new DeleteOfficeLocationCommand(id);

        await _mediator.Send(command);

        return Ok(new DeleteTeamResponse(id));
    }
}
