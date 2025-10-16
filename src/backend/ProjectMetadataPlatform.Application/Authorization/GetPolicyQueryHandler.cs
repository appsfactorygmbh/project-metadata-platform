using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Casbin;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Authorization;

/// <summary>
/// Handler for the <see cref="GetPolicyQuery" />
/// </summary>
public class GetPolicyQueryHandler : IRequestHandler<GetPolicyQuery, IEnumerable<string>>
{
    private readonly IEnforcerWrapper _enforcer;

    /// <summary>
    /// Creates a new Instance of  <see cref="GetPolicyQueryHandler"/>"
    /// </summary>
    /// <param name="enforcer">Authorization Enforcer</param>
    public GetPolicyQueryHandler(IEnforcerWrapper enforcer)
    {
        _enforcer = enforcer;
    }

    /// <summary>
    /// Gets all Policy rules.
    /// </summary>
    /// <param name="request">Request to get the authorization policy.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Array of policy rules.</returns>
    public async Task<IEnumerable<string>> Handle(
        GetPolicyQuery request,
        CancellationToken cancellationToken
    )
    {
        await _enforcer.LoadPolicyAsync();
        var policy = _enforcer.GetPolicy();

        return policy.Select(rule => string.Join(" && ", [.. rule]));
    }
}
