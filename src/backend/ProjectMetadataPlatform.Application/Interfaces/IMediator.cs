using System.Threading;
using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Interface for an Request Mediator
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends an Request to a Handler
    /// </summary>
    /// <typeparam name="TRequest">Type of the Request</typeparam>
    /// <typeparam name="TResult">Type of the Result</typeparam>
    /// <param name="request">Request to be handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Request Result.</returns>
    Task<TResult> Send<TRequest, TResult>(
        TRequest request,
        CancellationToken cancellationToken = default
    )
        where TRequest : IRequest<TResult>;

    /// <summary>
    /// Sends an Request without an Response to a Handler.
    /// </summary>
    /// <typeparam name="TRequest">Type of the Request</typeparam>
    /// <param name="request">Request to be handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest;
}
