using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Casbin;
using Casbin.Persist;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.VisualBasic;

namespace ProjectMetadataPlatform.Application.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnforcer _enforcer;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationBehavior(IEnforcer enforcer, IHttpContextAccessor httpContextAccessor)
    {
        _enforcer = enforcer;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var user = _httpContextAccessor.HttpContext.User;
        var requestType = typeof(TRequest).Name;

        if (requestType.EndsWith("Command"))
        {
            await AuthorizeCommandAsync(user, request);
        }

        var response = await next();

        if (requestType.EndsWith("Query"))
        {
            if (typeof(TResponse).Name == typeof(IEnumerable<>).Name)
            {
                await AuthorizeGetAllAsync(user, response);
            }
            else
            {
                await AuthorizeGetAsync(user, response);
            }
        }

        return response;
    }

    private async Task AuthorizeCommandAsync(ClaimsPrincipal? user, TRequest request)
    {
        if (!await _enforcer.EnforceAsync(user, request, "", typeof(TRequest).Name))
        {
            throw new System.Exception();
        }
        ;
    }

    private async Task AuthorizeGetAsync(ClaimsPrincipal? user, TResponse response)
    {
        if (!await _enforcer.EnforceAsync(user, response, "", typeof(TRequest).Name))
        {
            throw new System.Exception();
        }
        ;
    }

    private async Task AuthorizeGetAllAsync(ClaimsPrincipal? user, TResponse response)
    {
        foreach (var responseobject in (IEnumerable)response!)
        {
            if (!await _enforcer.EnforceAsync(user, responseobject, "", typeof(TRequest).Name))
            {
                throw new System.Exception();
            }
        }
    }
}
