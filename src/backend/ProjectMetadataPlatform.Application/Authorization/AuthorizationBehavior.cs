using System.Threading;
using System.Threading.Tasks;
using Casbin;
using MediatR;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.VisualBasic;

namespace ProjectMetadataPlatform.Application.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnforcer _enforcer;

    public AuthorizationBehavior(IEnforcer enforcer)
    {
        _enforcer = enforcer;
    }
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var type = typeof(TRequest).Name;
        var response = await next();

        return response;
    }
}
