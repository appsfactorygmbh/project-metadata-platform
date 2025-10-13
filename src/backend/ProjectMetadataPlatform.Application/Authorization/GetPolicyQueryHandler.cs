using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Casbin;
using MediatR;

namespace ProjectMetadataPlatform.Application.Authorization;

public class GetPolicyQueryHandler : IRequestHandler<GetPolicyQuery, IEnumerable<string>>
{
    private readonly IEnforcer _enforcer;

    public GetPolicyQueryHandler(IEnforcer enforcer)
    {
        _enforcer = enforcer;
    }

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
