using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Plugins.Models;
using ProjectMetadataPlatform.Api.Projects.Models;
using ProjectMetadataPlatform.Api.Teams.Models;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Api.Projects;

/// <summary>
/// Endpoints for managing projects.
/// </summary>
[ApiController]
[Authorize]
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
    /// <returns>All projects or all projects that match the given search string or filters.</returns>
    /// <response code="200">The projects are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetProjectsResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetProjectsResponse>>> Get(
        [FromQuery] ProjectFilterRequest? request,
        string? search = " "
    )
    {
        var query = new GetAllProjectsQuery(request, search);
        var projects = await _mediator.Send(query);
        var response = projects.Select(project => new GetProjectsResponse(
            Id: project.Id,
            Slug: project.Slug,
            ProjectName: project.ProjectName,
            ClientName: project.ClientName,
            IsArchived: project.IsArchived,
            Company: project.Company,
            Team: project.Team == null
                ? null
                : new()
                {
                    Id = project.Team.Id,
                    TeamName = project.Team.TeamName,
                    BusinessUnit = project.Team.BusinessUnit,
                    PTL = project.Team.PTL,
                },
            IsmsLevel: project.IsmsLevel,
            Notes: project.Notes
        ));
        return Ok(response);
    }

    /// <summary>
    /// Gets the project with the given slug.
    /// </summary>
    /// <param name="slug">The slug of the project.</param>
    /// <returns> The project.</returns>
    /// <response code="200">The Project is returned successfully.</response>
    /// <response code="404">The project could not be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(GetProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProjectResponse>> Get(string slug)
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
    [ProducesResponseType(typeof(GetProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProjectResponse>> Get(int id)
    {
        var query = new GetProjectQuery(id);
        var project = await _mediator.Send(query);

        var response = new GetProjectResponse(
            Id: project.Id,
            Slug: project.Slug,
            OfferId: project.OfferId,
            CompanyState: project.CompanyState,
            ProjectName: project.ProjectName,
            ClientName: project.ClientName,
            IsArchived: project.IsArchived,
            Company: project.Company,
            Team: project.Team == null
                ? null
                : new()
                {
                    Id = project.Team.Id,
                    TeamName = project.Team.TeamName,
                    BusinessUnit = project.Team.BusinessUnit,
                    PTL = project.Team.PTL,
                },
            IsmsLevel: project.IsmsLevel,
            Notes: project.Notes
        );

        return Ok(response);
    }

    /// <summary>
    /// Gets all the plugins of the project with the given id.
    /// </summary>
    /// <param name="id">The id of the project.</param>
    /// <returns>The plugins of the project.</returns>
    /// <response code="200">All Plugins of the project are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{id:int}/plugins")]
    [ProducesResponseType(typeof(IEnumerable<GetPluginResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetPluginResponse>>> GetPlugins(int id)
    {
        var query = new GetAllPluginsForProjectIdQuery(id);
        var projectPlugins = await _mediator.Send(query);

        var response = projectPlugins.Select(plugin => new GetPluginResponse(
            plugin.Plugin!.PluginName,
            plugin.Url,
            plugin.DisplayName ?? plugin.Plugin.PluginName,
            plugin.Plugin.Id
        ));

        return Ok(response);
    }

    /// <summary>
    /// Gets all the plugins of the project with the given id.
    /// </summary>
    /// <param name="slug">The slug of the project.</param>
    /// <returns>The plugins of the project.</returns>
    /// <response code="200">All Plugins of the project are returned successfully.</response>
    /// <response code="404">No project with the given Slug could be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{slug}/plugins")]
    [ProducesResponseType(typeof(IEnumerable<GetPluginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetPluginResponse>>> GetPluginsBySlug(string slug)
    {
        var projectId = await GetProjectId(slug);
        return await GetPlugins(projectId);
    }

    /// <summary>
    ///     Gets all the unarchived plugins of the project with the given id.
    /// </summary>
    /// <param name="id">The id of the project.</param>
    /// <returns>The unarchived plugins of the project.</returns>
    /// <response code="200">Returns the list of unarchived plugins for the project</response>
    /// <response code="404">If the project with the specified ID is not found</response>
    /// <response code="500">If there was an internal server error while processing the request</response>
    [HttpGet("{id:int}/unarchivedPlugins")]
    [ProducesResponseType(typeof(IEnumerable<GetPluginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetPluginResponse>>> GetUnarchivedPlugins(int id)
    {
        var query = new GetAllUnarchivedPluginsForProjectIdQuery(id);
        var unarchivedProjectPlugins = await _mediator.Send(query);

        var response = unarchivedProjectPlugins
            .Where(plugin => plugin.Plugin != null)
            .Select(plugin => new GetPluginResponse(
                plugin.Plugin!.PluginName,
                plugin.Url,
                plugin.DisplayName ?? plugin.Plugin.PluginName,
                plugin.Plugin.Id
            ));

        return Ok(response);
    }

    /// <summary>
    /// Gets all the unarchived plugins of the project with the given slug.
    /// </summary>
    /// <param name="slug">The slug of the project.</param>
    /// <returns>The unarchived plugins of the project.</returns>
    /// <response code="200">All unarchived plugins of the project are returned successfully.</response>
    /// <response code="404">No project with the given slug could be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{slug}/unarchivedPlugins")]
    [ProducesResponseType(typeof(IEnumerable<GetPluginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetPluginResponse>>> GetUnarchivedPluginsBySlug(
        string slug
    )
    {
        var projectId = await GetProjectId(slug);
        return await GetUnarchivedPlugins(projectId);
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

        IRequest<int> command =
            projectId == null
                ? new CreateProjectCommand(
                    ProjectName: projectRequest.ProjectName,
                    ClientName: projectRequest.ClientName,
                    OfferId: projectRequest.OfferId,
                    Company: projectRequest.Company,
                    CompanyState: projectRequest.CompanyState,
                    TeamId: projectRequest.TeamId,
                    IsmsLevel: projectRequest.IsmsLevel,
                    Plugins: (projectRequest.PluginList ?? [])
                        .Select(p => new ProjectPlugins
                        {
                            PluginId = p.Id,
                            DisplayName = p.DisplayName,
                            Url = p.Url,
                        })
                        .ToList(),
                    Notes: projectRequest.Notes
                )
                : new UpdateProjectCommand(
                    Id: projectId.Value,
                    ProjectName: projectRequest.ProjectName,
                    ClientName: projectRequest.ClientName,
                    OfferId: projectRequest.OfferId,
                    Company: projectRequest.Company,
                    CompanyState: projectRequest.CompanyState,
                    TeamId: projectRequest.TeamId,
                    IsmsLevel: projectRequest.IsmsLevel,
                    Plugins: (projectRequest.PluginList ?? [])
                        .Select(p => new ProjectPlugins
                        {
                            ProjectId = projectId.Value,
                            PluginId = p.Id,
                            DisplayName = p.DisplayName,
                            Url = p.Url,
                        })
                        .ToList(),
                    IsArchived: projectRequest.IsArchived,
                    Notes: projectRequest.Notes
                );

        var id = await _mediator.Send(command);

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
        _ = await _mediator.Send(command);
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
        return await _mediator.Send(query);
    }
}
