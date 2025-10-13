using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Casbin;
using MediatR;
using Microsoft.AspNetCore.Http;
using ProjectMetadataPlatform.Domain.Errors.UserException;

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
        var user = AuthorizationSubject.ConvertClaimsToAuthorizationSubject(
            _httpContextAccessor.HttpContext.User
        );
        var requestType = typeof(TRequest).Name;

        if (requestType.EndsWith("Command"))
        {
            await AuthorizeCommandAsync(user, request);
        }

        var response = await next();

        if (
            requestType != "LoginQuery"
            && requestType != "RefreshTokenQuery"
            && requestType.EndsWith("Query")
        )
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

    private async Task AuthorizeCommandAsync(AuthorizationSubject user, TRequest request)
    {
        if (!await _enforcer.EnforceAsync(user, request, "", typeof(TRequest).Name))
        {
            throw new UserUnauthorizedException();
        }
        ;
    }

    private async Task AuthorizeGetAsync(AuthorizationSubject user, TResponse response)
    {
        if (!await _enforcer.EnforceAsync(user, response, "", typeof(TRequest).Name))
        {
            throw new UserUnauthorizedException();
        }
        ;
    }

    private async Task AuthorizeGetAllAsync(AuthorizationSubject user, TResponse response)
    {
        var test = _enforcer.GetPolicy();
        foreach (var responseobject in (IEnumerable)response!)
        {
            if (!await _enforcer.EnforceAsync(user, responseobject, "", typeof(TRequest).Name))
            {
                throw new UserUnauthorizedException();
            }
        }
    }
}
