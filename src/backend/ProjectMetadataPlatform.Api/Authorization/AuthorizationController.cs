using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Authorization.Helper;
using ProjectMetadataPlatform.Api.Authorization.Models;
using ProjectMetadataPlatform.Application.Authorization;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.Authorization;

/// <summary>
/// Endpoints for managing authorization.
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="AuthorizationController" />.
    /// </summary>
    /// <param name="mediator"></param>
    public AuthorizationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns a human readable permissions filter for every action.
    /// </summary>
    /// <param name="resourceKind">The Kind of Resource Permissions should be returned for.</param>
    /// <returns>List of Permissions</returns>
    /// <response code="200">The permissions are returned successfully.</response>
    [HttpGet("{resourceKind}")]
    [ProducesResponseType(typeof(IEnumerable<GetPermissionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetPermissionResponse>>> GetPermissions(
        string resourceKind
    )
    {
        var query = new GetPermissionsQuery(resourceKind);
        var result = await _mediator.Send<
            GetPermissionsQuery,
            Dictionary<AuthorizationConstants.Actions, FilterTree>
        >(query);
        var response = result.Select(permission => new GetPermissionResponse(
            permission.Key,
            permission.Value.ToFilterResponse()
        ));

        return Ok(response);
    }

    /// <summary>
    /// Returns a list of all resources that resource policies exist for.
    /// </summary>
    /// <returns>List of resource names.</returns>
    /// <response code="200">The Resources are returned successfully.</response>
    [HttpGet("Resources")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetResources()
    {
        var query = new GetResourcesQuery();
        var result = await _mediator.Send<GetResourcesQuery, IEnumerable<string>>(query);
        return Ok(result);
    }
}
