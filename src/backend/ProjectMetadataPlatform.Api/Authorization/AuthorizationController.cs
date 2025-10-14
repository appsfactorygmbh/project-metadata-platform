using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Authorization.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Application.Authorization;

namespace ProjectMetadataPlatform.Api.Authorization;

/// <summary>
/// Endpoints for managing Authorization.
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = "Azure,Basic")]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="AuthorizationController"/>.
    /// </summary>
    /// <param name="mediator">The mediator instance for handling requests.</param>
    public AuthorizationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets the Authorization Policy.
    /// </summary>
    /// <returns>The Policy as a List of Rules.</returns>
    [HttpGet("Policy")]
    [ProducesResponseType(typeof(PolicyResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PolicyResponse>> Get()
    {
        var query = new GetPolicyQuery();

        var policies = await _mediator.Send(query);
        return new PolicyResponse(policies);
    }

    /// <summary>
    /// Creates a new Policy Rule.
    /// </summary>
    /// <param name="putRuleRequest">Policy Rule to be created.</param>
    /// <returns></returns>
    [HttpPut("Rule")]
    [ProducesResponseType(typeof(PolicyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Put([FromBody] PutRuleRequest putRuleRequest)
    {
        var query = new PutRuleCommand(putRuleRequest.PolicyRule);
        var result = await _mediator.Send(query);
        return result ? new CreatedResult("/Policy/", null) : new BadRequestResult();
    }
}
