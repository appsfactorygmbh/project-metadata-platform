using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Application.Authorization;

/// <summary>
/// Pipeline Behavior that checks whether authorization was checked in a request.
/// </summary>
/// <typeparam name="TRequest">Request to be handled.</typeparam>
/// <typeparam name="TResponse">Response from Handler.</typeparam>
public class AuthorizationEnforcerBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IAuthorizationTracker _tracker;

    /// <summary>
    /// Constructor for <see cref="AuthorizationEnforcerBehavior{TRequest,TResponse}"/>
    /// </summary>
    /// <param name="tracker">Tracks Authorization Status.</param>
    public AuthorizationEnforcerBehavior(IAuthorizationTracker tracker)
    {
        _tracker = tracker;
    }

    /// <summary>
    /// Checks if Authorization was checked in the Request and resets it.
    /// </summary>
    /// <param name="request">Request to be handled.</param>
    /// <param name="next">Next Pipeline Step.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response for the Request.</returns>
    /// <exception cref="UnauthorizedException">Thrown if no Authorization check happened.</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var response = await next(cancellationToken);
        if (_tracker.WasChecked)
        {
            _tracker.RevertCheck();
            return response;
        }
        else
        {
            throw new UnauthorizedException();
        }
    }
}
