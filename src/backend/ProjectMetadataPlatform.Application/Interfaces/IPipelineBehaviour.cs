using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Interface for Pipelines called before / after Requesthandlers.
/// </summary>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public interface IPipelineBehavior<in TInput, TOutput>
{
    /// <summary>
    /// Pipeline Step
    /// </summary>
    /// <param name="input"> Request for the Requesthandler.</param>
    /// <param name="nextStep">Next Pipeline Step.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>RequestHandler Response.</returns>
    Task<TOutput> Handle(
        TInput input,
        Func<Task<TOutput>> nextStep,
        CancellationToken cancellationToken = default
    );
}
