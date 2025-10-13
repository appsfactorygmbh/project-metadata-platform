using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Authorization.Models;
using ProjectMetadataPlatform.Application.Authorization;

namespace ProjectMetadataPlatform.Api.Authorization;

[ApiController]
[Authorize(AuthenticationSchemes = "Azure,Basic")]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthorizationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("Policy")]
    [ProducesResponseType(typeof(PolicyResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PolicyResponse>> Get()
    {
        var query = new GetPolicyQuery();

        var policies = await _mediator.Send(query);
        return new PolicyResponse(policies);
    }

    [HttpPut("Rule")]
    [ProducesResponseType(typeof(PolicyResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult> Put([FromBody] PutRuleRequest putRuleRequest)
    {
        var query = new PutRuleCommand(putRuleRequest.PolicyRule);
        await _mediator.Send(query);
        return new OkResult();
    }
}
