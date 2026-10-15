using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using Quartz;

namespace ProjectMetadataPlatform.Infrastructure.Jobs;

/// <summary>
/// Job for refreshing Materialized Search View.
/// </summary>
[DisallowConcurrentExecution]
public partial class RefreshSearchViewJob : IJob
{
    private readonly ProjectMetadataPlatformDbContext _context;
    private readonly ILogger<RefreshSearchViewJob> _logger;

    /// <summary>
    /// Constructor for <see cref="RefreshSearchViewJob"/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public RefreshSearchViewJob(
        ProjectMetadataPlatformDbContext context,
        ILogger<RefreshSearchViewJob> logger
    )
    {
        _context = context;
        _logger = logger;
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "An error occurred while executing the search view refresh job."
    )]
    private static partial void LogJobExecutionFailed(ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "Successfully refreshed materialized view mv_project_search. (Reported affected rows: {AffectedRows})"
    )]
    private static partial void LogJobExecutionSuccess(ILogger logger, int affectedRows);

    /// <summary>
    /// Executes Concurrent View Refresh
    /// </summary>
    /// <returns></returns>
    public async ValueTask Execute(
        IJobExecutionContext context,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var affectedRows = await _context.Database.ExecuteSqlRawAsync(
                "REFRESH MATERIALIZED VIEW CONCURRENTLY mv_project_search;",
                cancellationToken: cancellationToken
            );

            LogJobExecutionSuccess(_logger, affectedRows);
        }
        catch (Exception e)
        {
            LogJobExecutionFailed(_logger, e);
            throw new JobExecutionException(e);
        }
    }
}
