using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Mediator;

/// <summary>
/// Implementation of <see cref="IMediator" />
/// </summary>
public class Mediator(IServiceProvider provider) : IMediator
{
    /// <inheritdoc/>
    public async Task<TResult> Send<TRequest, TResult>(
        TRequest request,
        CancellationToken cancellationToken = default
    )
        where TRequest : IRequest<TResult>
    {
        var handler =
            provider.GetService<IRequestHandler<TRequest, TResult>>()
            ?? throw new InvalidOperationException(
                $"No handler registered for {request.GetType().Name}"
            );

        var behaviors = provider.GetServices<IPipelineBehavior<TRequest, TResult>>().Reverse();

        Func<Task<TResult>> handlerDelegate = () => handler.Handle(request, cancellationToken);
        foreach (var behavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = () => behavior.Handle(request, next, cancellationToken);
        }

        return await handlerDelegate();
    }

    /// <inheritdoc/>
    public async Task Send<TRequest>(
        TRequest request,
        CancellationToken cancellationToken = default
    )
        where TRequest : IRequest
    {
        _ = await Send<TRequest, Unit>(request, cancellationToken);
    }
}
