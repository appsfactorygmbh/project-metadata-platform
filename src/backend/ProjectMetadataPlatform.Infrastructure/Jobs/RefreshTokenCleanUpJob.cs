using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Interfaces;
using Quartz;

namespace ProjectMetadataPlatform.Infrastructure.Jobs;

/// <summary>
/// Job for removing expired Refresh Tokens
/// </summary>
public class RefreshTokenCleanUpJob : IJob
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Constructer for <see cref="RefreshTokenCleanUpJob"/>
    /// </summary>
    /// <param name="mediator"></param>
    public RefreshTokenCleanUpJob(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Executes <see cref="RefreshTokenCleanUpJob"/>
    /// </summary>
    /// <returns></returns>
    public async ValueTask Execute(
        IJobExecutionContext context,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _mediator.Send(new CleanUpRefreshTokensCommand(), cancellationToken);
        }
        catch (Exception e)
        {
            throw new JobExecutionException(e);
        }
    }
}
