using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Interfaces;
using Quartz;

namespace ProjectMetadataPlatform.Infrastructure.Jobs;

/// <summary>
/// Job for removing expired Refresh Tokens
/// </summary>
public partial class RefreshTokenCleanUpJob : IJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<RefreshTokenCleanUpJob> _logger;

    /// <summary>
    /// Constructer for <see cref="RefreshTokenCleanUpJob"/>
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="logger"></param>
    public RefreshTokenCleanUpJob(IMediator mediator, ILogger<RefreshTokenCleanUpJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "An error occurred while executing the refresh token clean up job."
    )]
    private static partial void LogJobExecutionFailed(ILogger logger, Exception exception);

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
            LogJobExecutionFailed(_logger, e);
            Console.WriteLine(e.Message);
            throw new JobExecutionException(e);
        }
    }
}
