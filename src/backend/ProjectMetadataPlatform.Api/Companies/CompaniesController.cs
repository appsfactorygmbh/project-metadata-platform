using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Companies.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Teams.Models;
using ProjectMetadataPlatform.Application.Companies;
using ProjectMetadataPlatform.Domain.Auth;
using GetLinkedProjectsForCompanyResponse = ProjectMetadataPlatform.Api.Companies.Models.GetLinkedProjectsForCompanyResponse;

namespace ProjectMetadataPlatform.Api.Companies;

[ApiController]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
[Route("[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="CompaniesController" />.
    /// </summary>
    /// <param name="mediator"></param>
    public CompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetCompanyResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetCompanyResponse>>> Get()
    {
        var query = new GetAllCompaniesQuery();
        var companies = await _mediator.Send(query);
        var response = companies.Select(company => new GetCompanyResponse(
            Id: company.Id,
            CompanyName: company.CompanyName
        ));

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetCompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetCompanyResponse>>> Get(int id)
    {
        var query = new GetCompanyQuery(id);
        var company = await _mediator.Send(query);
        var response = new GetCompanyResponse(Id: company.Id, CompanyName: company.CompanyName);

        return Ok(response);
    }

    [HttpPut]
    [ProducesResponseType(typeof(CreateCompanyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateCompanyResponse>> Put(
        [FromBody] CreateCompanyRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.CompanyName))
        {
            return BadRequest(new ErrorResponse("Company Name can't be empty or whitespaces"));
        }
        var command = new CreateCompanyCommand(CompanyName: request.CompanyName);

        var companyId = await _mediator.Send(command);

        var response = new CreateTeamResponse(companyId);
        var uri = "Companies/" + companyId;
        return Created(uri, response);
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(GetCompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetCompanyResponse>> Patch(
        int id,
        [FromBody] UpdateCompanyRequest request
    )
    {
        var command = new UpdateCompanyCommand(Id: id, CompanyName: request.CompanyName);
        var company = await _mediator.Send(command);

        var response = new GetCompanyResponse(Id: company.Id, CompanyName: company.CompanyName);

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(DeleteCompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeleteCompanyResponse>> Delete(int id)
    {
        var command = new DeleteCompanyCommand(id);

        await _mediator.Send(command);

        return Ok(new DeleteTeamResponse(id));
    }

    [HttpGet("{id:int}/linkedProjects")]
    [ProducesResponseType(typeof(GetLinkedProjectsForCompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetLinkedProjectsForCompanyResponse>> GetLinkedProjects(int id)
    {
        var command = new GetLinkedProjectsQuery(id);

        var slugList = await _mediator.Send(command);

        return Ok(new GetLinkedProjectsForCompanyResponse(slugList));
    }
}
