using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Common.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.ProjectPlugins.Models;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.ProjectPlugins;
using ProjectMetadataPlatform.Application.ProjectPlugins.Models;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Api.ProjectPlugins;

/// <summary>
/// Endpoints for managing Project plugins.
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
[Route("/Projects")]
public class ProjectPluginsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="ProjectPluginsController" />.
    /// </summary>
    /// <param name="mediator"></param>
    public ProjectPluginsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all the plugins of the project with the given id.
    /// </summary>
    /// <param name="id">The id of the project.</param>
    /// <returns>The plugins of the project.</returns>
    /// <response code="200">All Plugins of the project are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{id:int}/plugins")]
    [ProducesResponseType(
        typeof(GetListResponse<GetProjectPluginResponse>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetListResponse<GetProjectPluginResponse>>> GetPlugins(int id)
    {
        var query = new GetAllPluginsForProjectIdQuery(id);
        var (plugins, globalPermissions) = await _mediator.Send<
            GetAllPluginsForProjectIdQuery,
            (IEnumerable<ProjectPluginPermissionModel>, IEnumerable<AuthorizationConstants.Actions>)
        >(query);

        var pluginResponse = plugins.Select(plugin => new GetProjectPluginResponse(
            plugin.Plugin.ProjectId,
            plugin.Plugin.Id,
            plugin.Plugin.Plugin!.PluginName,
            plugin.Plugin.Url,
            plugin.Plugin.DisplayName ?? plugin.Plugin.Plugin.PluginName,
            plugin.Plugin.Plugin.Id,
            [.. plugin.PluginPermissions],
            [.. plugin.BillingPermissions]
        ));

        var response = new GetListResponse<GetProjectPluginResponse>(
            [.. pluginResponse],
            [.. globalPermissions]
        );
        return Ok(response);
    }

    /// <summary>
    /// Gets all the plugins of the project with the given slug.
    /// </summary>
    /// <param name="slug">The slug of the project.</param>
    /// <returns>The plugins of the project.</returns>
    /// <response code="200">All Plugins of the project are returned successfully.</response>
    /// <response code="404">No project with the given Slug could be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{slug}/plugins")]
    [ProducesResponseType(
        typeof(GetListResponse<GetProjectPluginResponse>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetListResponse<GetProjectPluginResponse>>> GetPluginsBySlug(
        string slug
    )
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
    [ProducesResponseType(
        typeof(GetListResponse<GetProjectPluginResponse>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetListResponse<GetProjectPluginResponse>>> GetUnarchivedPlugins(
        int id
    )
    {
        var query = new GetAllUnarchivedPluginsForProjectIdQuery(id);
        var (plugins, globalPermissions) = await _mediator.Send<
            GetAllUnarchivedPluginsForProjectIdQuery,
            (IEnumerable<ProjectPluginPermissionModel>, IEnumerable<AuthorizationConstants.Actions>)
        >(query);

        var pluginResponse = plugins.Select(plugin => new GetProjectPluginResponse(
            plugin.Plugin.ProjectId,
            plugin.Plugin.Id,
            plugin.Plugin.Plugin!.PluginName,
            plugin.Plugin.Url,
            plugin.Plugin.DisplayName ?? plugin.Plugin.Plugin.PluginName,
            plugin.Plugin.Plugin.Id,
            [.. plugin.PluginPermissions],
            [.. plugin.BillingPermissions]
        ));

        var response = new GetListResponse<GetProjectPluginResponse>(
            [.. pluginResponse],
            [.. globalPermissions]
        );
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
    [ProducesResponseType(
        typeof(GetListResponse<GetProjectPluginResponse>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<
        ActionResult<GetListResponse<GetProjectPluginResponse>>
    > GetUnarchivedPluginsBySlug(string slug)
    {
        var projectId = await GetProjectId(slug);
        return await GetUnarchivedPlugins(projectId);
    }

    /// <summary>
    /// Adds a new Plugin to a Project.
    /// </summary>
    /// <param name="request">Plugin Creation request.</param>
    /// <param name="projectId">The id of the project.</param>
    /// <returns>Id of the newly created project.</returns>
    /// <response code="201">ProjectPlugin was created succesfully.</response>
    /// <response code="404">No project with the given Id could be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPut("{projectId:int}/plugins")]
    [ProducesResponseType(typeof(AddProjectPluginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AddProjectPluginResponse>> AddProjectPlugin(
        [FromBody] AddProjectPluginRequest request,
        int projectId
    )
    {
        return await ProcessAddProjectPlugin(request, projectId, projectId.ToString());
    }

    /// <summary>
    /// Adds a new Plugin to a Project identified by its slug.
    /// </summary>
    /// <param name="request">Project Plugin Creation request.</param>
    /// <param name="slug">The slug of the project.</param>
    /// <returns>Id of the newly created project.</returns>
    /// <response code="201">ProjectPlugin was created succesfully.</response>
    /// <response code="404">No project with the given Id could be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPut("{slug}/plugins")]
    [ProducesResponseType(typeof(AddProjectPluginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AddProjectPluginResponse>> AddProjectPluginBySlug(
        [FromBody] AddProjectPluginRequest request,
        string slug
    )
    {
        var projectId = await GetProjectId(slug);
        return await ProcessAddProjectPlugin(request, projectId, slug);
    }

    /// <summary>
    ///  Processes an AddProjectPluginRequest
    /// </summary>
    /// <param name="request">request to be processed.</param>
    /// <param name="projectId">Id of the project.</param>
    /// <param name="identifier">Id or Slug of the project (based on caller route).</param>
    /// <returns></returns>
    private async Task<ActionResult<AddProjectPluginResponse>> ProcessAddProjectPlugin(
        AddProjectPluginRequest request,
        int projectId,
        string identifier
    )
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest(new ErrorResponse("Url can't be empty or whitespaces"));
        }
        var command = new AddProjectPluginCommand(
            projectId,
            request.PluginId,
            request.DisplayName,
            request.Url
        );
        var id = await _mediator.Send<AddProjectPluginCommand, int>(command);

        var response = new AddProjectPluginResponse(id);
        return Created("/Projects/" + identifier + "/plugins/" + id, response);
    }

    /// <summary>
    /// Edits a Plugin of a Project.
    /// </summary>
    /// <param name="request">Update Request.</param>
    /// <param name="projectId">The id of the project.</param>
    /// <param name="pluginId">Id of the project plugin.</param>
    /// <returns>updated plugin.</returns>
    /// <response code="200">ProjectPlugin was updated succesfully.</response>
    /// <response code="404">No project or no plugin with the given Id could be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPatch("{projectId:int}/plugins/{pluginId:int}")]
    [ProducesResponseType(typeof(GetProjectPluginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProjectPluginResponse>> UpdateProjectPlugin(
        [FromBody] UpdateProjectPluginRequest request,
        int projectId,
        int pluginId
    )
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest(new ErrorResponse("Url can't be empty or whitespaces"));
        }
        var command = new UpdateProjectPluginCommand(
            projectId,
            pluginId,
            request.DisplayName,
            request.Url
        );
        var plugin = await _mediator.Send<UpdateProjectPluginCommand, ProjectPlugin>(command);

        var pluginResponse = new GetProjectPluginResponse(
            plugin.ProjectId,
            plugin.Id,
            plugin.Plugin!.PluginName,
            plugin.Url,
            plugin.DisplayName ?? plugin.Plugin.PluginName,
            plugin.PluginId
        );
        return Ok(pluginResponse);
    }

    /// <summary>
    /// Edits a Plugin of a Project via slug.
    /// </summary>
    /// <param name="request">Update request.</param>
    /// <param name="slug">The Slug of the project.</param>
    /// <param name="pluginId">Id of project plugin.</param>
    /// <returns>updated plugin.</returns>
    /// <response code="200">ProjectPlugin was updated succesfully.</response>
    /// <response code="404">No project or no plugin with the given Id/slug could be found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPatch("{slug}/plugins/{pluginId:int}")]
    [ProducesResponseType(typeof(UpdateProjectPluginRequest), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProjectPluginResponse>> UpdateProjectPluginBySlug(
        [FromBody] UpdateProjectPluginRequest request,
        string slug,
        int pluginId
    )
    {
        var projectId = await GetProjectId(slug);
        return await UpdateProjectPlugin(request, projectId, pluginId);
    }

    /// <summary>
    /// Deletes a project plugin of a project.
    /// </summary>
    /// <param name="projectId">Id of the project.</param>
    /// <param name="pluginId">Id of the project plugin.</param>
    /// <returns>No content Result.</returns>
    /// <response code="204"> Request Successfull.</response>
    /// <response code="404"> Object not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpDelete("{projectId:int}/plugins/{pluginId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProjectPlugin(int projectId, int pluginId)
    {
        var command = new DeleteProjectPluginCommand(projectId, pluginId);
        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Deletes a project plugin from a project identified by slug.
    /// </summary>
    /// <param name="slug">Slug of the project.</param>
    /// <param name="pluginId">Id of the project.</param>
    /// <returns>No Content Result.</returns>
    /// <response code="204"> Request Successfull.</response>
    /// <response code="404"> Object not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpDelete("{slug}/plugins/{pluginId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProjectPluginBySlug(string slug, int pluginId)
    {
        var projectId = await GetProjectId(slug);
        return await DeleteProjectPlugin(projectId, pluginId);
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
