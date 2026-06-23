using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Application.Authorization;

public class AuthorizationEnforcerBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull()
{
    private readonly IAuthorizationTracker _tracker;

    public AuthorizationEnforcerBehavior(IAuthorizationTracker tracker)
    {
        _tracker = tracker;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var response = await next(cancellationToken);
        if (_tracker.WasChecked)
        {
            return response;
        }
        else
        {
            throw new UnauthorizedException();
        }
    }
}
