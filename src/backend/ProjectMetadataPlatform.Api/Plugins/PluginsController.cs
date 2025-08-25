using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Plugins.Models;
using ProjectMetadataPlatform.Application.Plugins;

namespace ProjectMetadataPlatform.Api.Plugins;

/// <summary>
/// Endpoints for managing plugins.
/// </summary>
[ApiController]
[Authorize]
[Route("[controller]")]
public class PluginsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="PluginsController" />.
    /// </summary>
    /// <param name="mediator"></param>
    public PluginsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new plugin with the given name.
    /// </summary>
    /// <param name="request">The request body.</param>
    /// <returns>An HTTP Created Response and the Id of the new Plugin.</returns>
    /// <response code="201">The Plugin was created successfully.</response>
    /// <response code="400">The request data is invalid.</response>
    /// <response code="409">The Plugin name already exists.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPut]
    [ProducesResponseType(typeof(CreatePluginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreatePluginResponse>> Put(
        [FromBody] CreatePluginRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.PluginName))
        {
            return BadRequest(new ErrorResponse("PluginName can't be empty or whitespaces"));
        }

        var command = new CreatePluginCommand(
            request.PluginName,
            request.IsArchived,
            request.Keys,
            request.BaseUrl
        );

        var pluginId = await _mediator.Send(command);

        var response = new CreatePluginResponse(pluginId);
        var uri = "/Plugins/" + pluginId;
        return Created(uri, response);
    }

    /// <summary>
    /// Updates a global plugin.
    /// </summary>
    /// <param name="pluginId">The id of the plugin to update.</param>
    /// <param name="request">The request body containing the details of the global plugin to be updated.</param>
    /// <returns>The updated version of the Plugin.</returns>
    /// <response code="200">The Plugin was updated successfully.</response>
    /// <response code="404">No Plugin with the requested id was found.</response>
    /// <response code="409">The Plugin name already exists.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPatch("{pluginId:int}")]
    [ProducesResponseType(typeof(GetGlobalPluginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GetGlobalPluginResponse>> Patch(
        int pluginId,
        [FromBody] PatchGlobalPluginRequest request
    )
    {
        var command = new PatchGlobalPluginCommand(
            pluginId,
            request.PluginName,
            request.IsArchived,
            request.BaseUrl
        );

        var plugin = await _mediator.Send(command);

        var response = new GetGlobalPluginResponse(
            plugin.PluginName,
            plugin.Id,
            plugin.IsArchived,
            [],
            plugin.BaseUrl
        );
        return Ok(response);
    }

    /// <summary>
    /// Gets all global plugins.
    /// </summary>
    /// <returns>All global plugins.</returns>
    /// <response code="200">All global plugins are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetGlobalPluginResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetGlobalPluginResponse>>> GetGlobal()
    {
        var query = new GetGlobalPluginsQuery();
        var plugins = await _mediator.Send(query);

        string[] keys = [];
        var response = plugins.Select(plugin => new GetGlobalPluginResponse(
            plugin.PluginName,
            plugin.Id,
            plugin.IsArchived,
            keys,
            plugin.BaseUrl
        ));

        return Ok(response);
    }

    /// <summary>
    /// Deletes a plugin by its ID.
    /// </summary>
    /// <param name="pluginId">The unique identifier of the plugin to delete.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, with an <see cref="ActionResult"/> that
    /// contains a <see cref="DeleteGlobalPluginResponse"/> on success, or an error message on failure.
    /// </returns>
    /// <response code="200">OK if the plugin was successfully deleted.</response>
    /// <response code="400">Bad Request if the pluginId is smaller than or equal to 0, or if the plugin with the given id is not archived.</response>
    /// <response code="404">Not Found if no plugin with the specified ID was found.</response>
    /// <response code="500">Internal Server Error if an unexpected exception occurs.</response>
    [HttpDelete("{pluginId:int}")]
    [ProducesResponseType(typeof(DeleteGlobalPluginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeleteGlobalPluginResponse>> Delete(int pluginId)
    {
        if (pluginId <= 0)
        {
            return BadRequest(new ErrorResponse("PluginId can't be smaller than or equal to 0"));
        }
        var command = new DeleteGlobalPluginCommand(pluginId);

        _ = await _mediator.Send(command);

        return Ok(new DeleteGlobalPluginResponse(pluginId));
    }
}
