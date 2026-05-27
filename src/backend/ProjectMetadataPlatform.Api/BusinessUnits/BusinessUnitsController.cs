using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.BusinessUnits.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Teams.Models;
using ProjectMetadataPlatform.Application.BusinessUnits;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Api.BusinessUnits;

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

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetBusinessUnitResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetBusinessUnitResponse>>> Get()
    {
        var query = new GetAllBusinessUnitsQuery();
        var businessUnits = await _mediator.Send(query);
        var response = businessUnits.Select(businessUnit => new GetBusinessUnitResponse(
            Id: businessUnit.Id,
            BusinessUnitName: businessUnit.BusinessUnitName
        ));

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetBusinessUnitResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetBusinessUnitResponse>>> Get(int id)
    {
        var query = new GetBusinessUnitQuery(id);
        var businessUnit = await _mediator.Send(query);
        var response = new GetBusinessUnitResponse(
            Id: businessUnit.Id,
            BusinessUnitName: businessUnit.BusinessUnitName
        );

        return Ok(response);
    }

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

        var response = new CreateTeamResponse(businessUnitId);
        var uri = "BusinessUnits/" + businessUnitId;
        return Created(uri, response);
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(GetBusinessUnitResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetBusinessUnitResponse>> Patch(
        int id,
        [FromBody] UpdateBusinessUnitRequest request
    )
    {
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

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(DeleteBusinessUnitResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeleteBusinessUnitResponse>> Delete(int id)
    {
        var command = new DeleteBusinessUnitCommand(id);

        await _mediator.Send(command);

        return Ok(new DeleteTeamResponse(id));
    }

    [HttpGet("{id:int}/linkedTeams")]
    [ProducesResponseType(typeof(GetLinkedTeamsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetLinkedTeamsResponse>> GetLinkedTeams(int id)
    {
        var command = new GetLinkedTeamsQuery(id);

        var idList = await _mediator.Send(command);

        return Ok(new GetLinkedTeamsResponse(idList));
    }
}
