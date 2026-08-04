using System.Threading;
using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Represents a Handler for a Request.
/// </summary>
/// <typeparam name="TRequest">Type of the Request.</typeparam>
/// <typeparam name="TResult">Type of the Response.</typeparam>
public interface IRequestHandler<in TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    /// <summary>
    /// Handles an Request.
    /// </summary>
    /// <param name="request">Request to be handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response to the Request.</returns>
    Task<TResult> Handle(TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a Handler for a Request without a Response.
/// </summary>
/// <typeparam name="TRequest">Type of the Request.</typeparam>
public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit>
    where TRequest : IRequest
{
    /// <summary>
    /// Handles a Request without a Response.
    /// </summary>
    /// <param name="request">Request to be handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    new Task Handle(TRequest request, CancellationToken cancellationToken = default);

    async Task<Unit> IRequestHandler<TRequest, Unit>.Handle(
        TRequest request,
        CancellationToken cancellationToken
    )
    {
        await Handle(request, cancellationToken);
        return Unit.Value;
    }
}
