using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.BusinessUnits.Models;
using ProjectMetadataPlatform.Api.Common.Models;
using ProjectMetadataPlatform.Api.Companies.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Projects.Models;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Api.Projects;

/// <summary>
/// Endpoints for managing projects.
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
[Route("[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="ProjectsController" /> class.
    /// </summary>
    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all projects or all projects that match the given search string. Also orders response alphabetical by ClientName and then by ProjectName
    /// </summary>
    /// <param name="request">The collection of filters to search by.</param>
    /// <param name="search">Search string to filter the projects by.</param>
    /// <returns>All projects or all projects that match the given search string or filters with the allowed actions on the type.</returns>
    /// <response code="200">The projects are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(GetListResponse<GetProjectResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetListResponse<GetProjectResponse>>> Get(
        [FromQuery] ProjectFilterRequest? request,
        string? search = " "
    )
    {
        var query = new GetAllProjectsQuery(request, search);
        var (projects, permissions) = await _mediator.Send<
            GetAllProjectsQuery,
            (IEnumerable<Project>, IEnumerable<AuthorizationConstants.Actions>)
        >(query);
        var projectResponse = projects.Select(project => new GetProjectResponse(
            Id: project.Id,
            Slug: project.Slug,
            ProjectName: project.ProjectName,
            ClientName: project.ClientName,
            IsArchived: project.IsArchived,
            Company: new GetCompanyResponse(project.Company!.Id, project.Company!.CompanyName),
            Team: project.Team == null
                ? null
                : new()
                {
                    Id = project.Team.Id,
                    TeamName = project.Team.TeamName,
                    BusinessUnit = new GetBusinessUnitResponse(
                        project.Team.BusinessUnit!.Id,
                        project.Team.BusinessUnit!.BusinessUnitName
                    ),
                    PTL = project.Team.PTL,
                },
            IsmsLevel: project.IsmsLevel,
            IsEoC: project.IsEoC,
            Notes: project.Notes
        ));
        var response = new GetListResponse<GetProjectResponse>(
            [.. projectResponse],
            [.. permissions]
        );
        return Ok(response);
    }

    /// <summary>
    /// Gets the project with the given slug.
    /// </summary>
    /// <param name="slug">The slug of the project.</param>
    /// <returns> The project with actions allowed on it.</returns>
    /// <response code="200">The Project is returned successfully.</response>
    /// <response code="404">The project could not be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(GetProjectDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProjectDetailsResponse>> Get(string slug)
    {
        var projectId = await GetProjectId(slug);
        return await Get(projectId);
    }

    /// <summary>
    /// Gets the project with the given id.
    /// </summary>
    /// <param name="id">The id of the project.</param>
    /// <returns>The project.</returns>
    /// <response code="200">The Project is returned successfully.</response>
    /// <response code="404">The project could not be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetProjectDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProjectDetailsResponse>> Get(int id)
    {
        var query = new GetProjectQuery(id);
        var (project, permissions) = await _mediator.Send<
            GetProjectQuery,
            (Project, IEnumerable<AuthorizationConstants.Actions>)
        >(query);

        var response = new GetProjectDetailsResponse(
            Id: project.Id,
            Slug: project.Slug,
            CompanyState: project.CompanyState,
            ProjectName: project.ProjectName,
            ClientName: project.ClientName,
            IsArchived: project.IsArchived,
            Company: new GetCompanyResponse(project.Company!.Id, project.Company!.CompanyName),
            Team: project.Team == null
                ? null
                : new()
                {
                    Id = project.Team.Id,
                    TeamName = project.Team.TeamName,
                    BusinessUnit = new GetBusinessUnitResponse(
                        project.Team.BusinessUnit!.Id,
                        project.Team.BusinessUnit!.BusinessUnitName
                    ),
                    PTL = project.Team.PTL,
                },
            IsEoC: project.IsEoC,
            IsmsLevel: project.IsmsLevel,
            Notes: project.Notes,
            Permissions: [.. permissions]
        );

        return Ok(response);
    }

    /// <summary>
    /// Creates a new project or updates the one with given slug.
    /// </summary>
    /// <param name="project">The data of the new project.</param>
    /// <param name="slug">The slug, if an existing project should be overwritten.</param>
    /// <returns>A response containing the id of the created project.</returns>
    /// <response code="201">The Project has been created successfully.</response>
    /// <response code="400">The request data is invalid.</response>
    /// <response code="404">The project with the specified slug was not found.</response>
    /// <response code="409">The project with the slug generated from the name already exists.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPut("{slug}")]
    [ProducesResponseType(typeof(PutProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PutProjectResponse>> Put(
        [FromBody] PutProjectRequest project,
        string slug
    )
    {
        var projectId = await GetProjectId(slug);
        return await Put(project, projectId);
    }

    /// <summary>
    /// Creates a new project or updates the one with given id.
    /// </summary>
    /// <param name="projectRequest">The data of the new project.</param>
    /// <param name="projectId">The id, if an existing project should be overwritten.</param>
    /// <returns>A response containing the id of the created project.</returns>
    /// <response code="201">The Project has been created successfully.</response>
    /// <response code="400">The request data is invalid.</response>
    /// <response code="409">The project with the slug generated from the name already exists.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPut]
    [ProducesResponseType(typeof(PutProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PutProjectResponse>> Put(
        [FromBody] PutProjectRequest projectRequest,
        int? projectId = null
    )
    {
        if (
            string.IsNullOrWhiteSpace(projectRequest.ProjectName)
            || string.IsNullOrWhiteSpace(projectRequest.ClientName)
        )
        {
            return BadRequest(new ErrorResponse("ProjectName and ClientName must not be empty."));
        }

        int id;
        if (projectId == null)
        {
            var command = new CreateProjectCommand(
                ProjectName: projectRequest.ProjectName,
                ClientName: projectRequest.ClientName,
                CompanyId: projectRequest.CompanyId,
                CompanyState: projectRequest.CompanyState,
                TeamId: projectRequest.TeamId,
                IsmsLevel: projectRequest.IsmsLevel,
                IsEoC: projectRequest.IsEoC,
                Notes: projectRequest.Notes
            );
            id = await _mediator.Send<CreateProjectCommand, int>(command);
        }
        else
        {
            var command = new UpdateProjectCommand(
                Id: projectId.Value,
                ProjectName: projectRequest.ProjectName,
                ClientName: projectRequest.ClientName,
                CompanyId: projectRequest.CompanyId,
                CompanyState: projectRequest.CompanyState,
                TeamId: projectRequest.TeamId,
                IsmsLevel: projectRequest.IsmsLevel,
                IsArchived: projectRequest.IsArchived,
                IsEoC: projectRequest.IsEoC,
                Notes: projectRequest.Notes
            );
            id = await _mediator.Send<UpdateProjectCommand, int>(command);
        }

        var response = new PutProjectResponse(id);
        return Created("/Projects/" + id, response);
    }

    /// <summary>
    /// Deletes the project with the given slug.
    /// </summary>
    /// <param name="slug">The slug of the project to delete.</param>
    /// <returns>An ActionResult indicating the result of the delete operation.</returns>
    /// <response code="204">The project was deleted successfully.</response>
    /// <response code="400">The request was invalid.</response>
    /// <response code="404">The project with the specified slug was not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpDelete("{slug}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(string slug)
    {
        var projectId = await GetProjectId(slug);
        return await Delete(projectId);
    }

    /// <summary>
    /// Deletes the project with the given id.
    /// </summary>
    /// <param name="id">The id of the project to delete.</param>
    /// <returns>An ActionResult indicating the result of the delete operation.</returns>
    /// <response code="204">The project was deleted successfully.</response>
    /// <response code="400">The request was invalid.</response>
    /// <response code="404">The project with the specified id was not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteProjectCommand(id);
        _ = await _mediator.Send<DeleteProjectCommand, Project?>(command);
        return NoContent();
    }

    /// <summary>
    /// Gets a project id by its slug.
    /// </summary>
    /// <param name="slug">The slug of the project.</param>
    /// <returns>The id of the project.</returns>
    private async Task<int> GetProjectId(string slug)
    {
        var query = new GetProjectIdBySlugQuery(slug);
        return await _mediator.Send<GetProjectIdBySlugQuery, int>(query);
    }
}
