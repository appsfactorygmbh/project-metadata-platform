using System.Collections.Generic;
using System.Linq;
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

/// <summary>
/// Endpoints for managing Companies.
/// </summary>
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

    /// <summary>
    /// Returns all Companies
    /// </summary>
    /// <returns>List of Companies</returns>
    /// <response code="200"> Companies are returned succesfully. </response>
    /// <response code="500"> Internal Error. </response>
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

    /// <summary>
    /// Returns the specified Company.
    /// </summary>
    /// <param name="id">Id of the specified Company.</param>
    /// <returns>A Company.</returns>1
    /// <response code="200"> Company was returned succesfully.</response>
    /// <response code="404">Company could not be found. </response>
    /// <response code="500"> Internal error. </response>
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

    /// <summary>
    /// Creates a new Company.
    /// </summary>
    /// <param name="request"> Request to create a Company.</param>
    /// <returns> Id of the newly created company. </returns>
    /// <response code="201"> Company was created succesfully. </response>
    /// <response code="400"> Company could not be created. </response>
    /// <response code="409"> Company name already exists. </response>
    /// <response code="500"> Internal error. </response>
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

        var response = new CreateCompanyResponse(companyId);
        var uri = "Companies/" + companyId;
        return Created(uri, response);
    }

    /// <summary>
    /// Updates a Company.
    /// </summary>
    /// <param name="id"> Id of the Company. </param>
    /// <param name="request">Update request. </param>
    /// <returns>The Updated Company. </returns>
    /// <response code="200"> Company was updated succesfully. </response>
    /// <response code="400"> Company could not be updated. </response>
    /// <response code="404"> Company could not be found. </response>
    /// <response code="409"> Updated Company Name already exists. </response>
    /// <response code="500"> Internal Error. </response>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(GetCompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetCompanyResponse>> Patch(
        int id,
        [FromBody] UpdateCompanyRequest request
    )
    {
        if (request.CompanyName != null && string.IsNullOrWhiteSpace(request.CompanyName))
        {
            return BadRequest(new ErrorResponse("Company Name can't be whitespaces"));
        }
        var command = new UpdateCompanyCommand(Id: id, CompanyName: request.CompanyName);
        var company = await _mediator.Send(command);

        var response = new GetCompanyResponse(Id: company.Id, CompanyName: company.CompanyName);

        return Ok(response);
    }

    /// <summary>
    /// Deletes a Company.
    /// </summary>
    /// <param name="id"> ID of the company. </param>
    /// <returns>No Content.</returns>
    /// <response code="204"> Company was deleted. </response>
    /// <response code="400"> Company could not be deleted. </response>
    /// <response code="404"> Company could not be found. </response>
    /// <response code="500"> Internal Error. </response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteCompanyCommand(id);

        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Returns slugs for all projects linked to the specified Company-
    /// </summary>
    /// <param name="id">Id of the Company. </param>
    /// <returns> List of Project Slugs. </returns>
    /// <response code="200"> Slugs are returned succesfully. </response>
    /// <response code="404"> Company could not be found. </response>
    /// <response code="500"> Internal Error. </response>
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
