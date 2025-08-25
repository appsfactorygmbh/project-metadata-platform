using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Api.Logs.Models;
using ProjectMetadataPlatform.Application.Logs;
using ProjectMetadataPlatform.Application.Projects;

namespace ProjectMetadataPlatform.Api.Logs;

/// <summary>
/// Endpoints for managing log entries.
/// </summary>
[ApiController]
[Authorize]
[Route("[controller]")]
public class LogsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogConverter _converter;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for handling requests.</param>
    /// <param name="converter">The log converter instance for converting log entries.</param>
    public LogsController(IMediator mediator, ILogConverter converter)
    {
        _mediator = mediator;
        _converter = converter;
    }

    /// <summary>
    /// Retrieves log entries based on the specified project ID and search criteria.
    /// Filters are optional and can *not* be used in combination.
    /// if multiple filters are used, the first one will be used.
    /// projectId > search > userId > globalPluginId
    /// </summary>
    /// <param name="projectId">The ID of the project to filter logs by.</param>
    /// <param name="search">The search term to filter logs by.</param>
    /// <param name="userId">The ID of the affected user to filter logs by.</param>
    /// <param name="globalPluginId">The ID of the global plugin to filter logs by.</param>
    /// <param name="projectSlug">The slug of the project to filter logs by.</param>
    /// <returns>A list of log responses.</returns>
    /// <response code="200">Returns the list of log responses.</response>
    /// <response code="404">Not Project with the given id was found.</response>
    /// <response code="500">If an error occurs while processing the request.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<LogResponse>>> Get(
        int? projectId,
        string? search,
        string? userId,
        int? globalPluginId,
        string? projectSlug
    )
    {
        var projectFromSlugId = (int?)null;

        if (projectSlug != null && projectId == null)
        {
            var projectIdFromSlugQuery = new GetProjectIdBySlugQuery(projectSlug);
            projectFromSlugId = await _mediator.Send(projectIdFromSlugQuery);
        }

        var query = new GetLogsQuery(
            projectId ?? projectFromSlugId,
            search,
            userId,
            globalPluginId
        );

        var logs = await _mediator.Send(query);

        return Ok(logs.Select(_converter.BuildLogMessage));
    }
}
