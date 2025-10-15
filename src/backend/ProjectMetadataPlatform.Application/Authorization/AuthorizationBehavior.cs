using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Casbin;
using MediatR;
using Microsoft.AspNetCore.Http;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.UserException;

namespace ProjectMetadataPlatform.Application.Authorization;

/// <summary>
/// Pipeline Behavior that checks Authorization before commands are handled and before queries are returned.
/// </summary>
/// <typeparam name="TRequest">Request to be handled.</typeparam>
/// <typeparam name="TResponse">Response from Handler.</typeparam>
public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnforcer _enforcer;

    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Creates a new Instance of <see cref="AuthorizationBehavior{TRequest,TResponse}"/> "
    /// </summary>
    public AuthorizationBehavior(IEnforcer enforcer, IHttpContextAccessor httpContextAccessor)
    {
        _enforcer = enforcer;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Checks Authorization.
    /// </summary>
    /// <param name="request">Request to be handled.</param>
    /// <param name="next">Next Pipeline step.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response for the Request.</returns>
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

    /// <summary>
    /// Enforces Authorization for a Subject for a Request
    /// </summary>
    /// <param name="user">Subject requesting access.</param>
    /// <param name="request">Request to update or create an object.</param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException">Thrown if the access is unauthorized</exception>
    private async Task AuthorizeCommandAsync(AuthorizationSubject user, TRequest request)
    {
        await _enforcer.LoadPolicyAsync();
        if (!await _enforcer.EnforceAsync(user, request, "", typeof(TRequest).Name))
        {
            throw new UnauthorizedException();
        }
        ;
    }

    /// <summary>
    /// Enforces Authorization for Subject requesting access to a object.
    /// </summary>
    /// <param name="user">Subject requesting access.</param>
    /// <param name="response">Requested object.</param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException">Thrown if the access is unauthorized</exception>
    private async Task AuthorizeGetAsync(AuthorizationSubject user, TResponse response)
    {
        await _enforcer.LoadPolicyAsync();
        if (!await _enforcer.EnforceAsync(user, response, "", typeof(TRequest).Name))
        {
            throw new UnauthorizedException();
        }
        ;
    }

    /// <summary>
    /// Enforces Authorization for Subject requesting access to a list of objects.
    /// </summary>
    /// <param name="user">Subject requesting access.</param>
    /// <param name="response">Requested list of objects.</param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException">>Thrown if the access is unauthorized</exception>
    private async Task AuthorizeGetAllAsync(AuthorizationSubject user, TResponse response)
    {
        await _enforcer.LoadPolicyAsync();

        foreach (dynamic responseobject in (IEnumerable)response!)
        {
            if (!await EnforceDynamic(user, responseobject, typeof(TRequest).Name))
            {
                throw new UnauthorizedException();
            }
        }
    }

    private async Task<bool> EnforceDynamic<TResponseItem>(
        AuthorizationSubject user,
        TResponseItem responseItem,
        string requestName
    )
    {
        return await _enforcer.EnforceAsync(user, responseItem, "", requestName);
    }
}
