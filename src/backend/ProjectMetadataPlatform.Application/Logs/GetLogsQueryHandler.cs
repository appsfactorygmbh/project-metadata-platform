using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Logs;

/// <summary>
/// Handles the query to retrieve logs based on project ID and search criteria.
/// </summary>
public class GetLogsQueryHandler : IRequestHandler<GetLogsQuery, (IEnumerable<Log>, LogCursor?)>
{
    private readonly ILogRepository _logRepository;
    private readonly IPaginationHelper _paginationHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetLogsQueryHandler"/> class.
    /// </summary>
    /// <param name="logRepository">The log repository instance.</param>
    /// <param name="paginationHelper"></param>
    public GetLogsQueryHandler(ILogRepository logRepository, IPaginationHelper paginationHelper)
    {
        _logRepository = logRepository;
        _paginationHelper = paginationHelper;
    }

    /// <summary>
    /// Handles the GetLogsQuery request.
    /// Filters are optional and can *not* be used in combination.
    /// if multiple filters are used, the first one will be used.
    /// projectId > search > userId > globalPluginId
    /// </summary>
    /// <param name="request">The request containing project ID and search criteria.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of log responses.</returns>
    public async Task<(IEnumerable<Log>, LogCursor?)> Handle(
        GetLogsQuery request,
        CancellationToken cancellationToken
    )
    {
        var logs = request switch
        {
            { ProjectId: { } projectId } => await _logRepository.GetLogsForProject(
                projectId,
                request.Cursor
            ),
            { Search: { } search } => await _logRepository.GetLogsWithSearch(
                search,
                request.StartDate,
                request.EndDate,
                request.Cursor
            ),
            { UserId: { } userId } => await _logRepository.GetLogsForUser(userId, request.Cursor),
            { GlobalPluginId: { } globalPluginId } => await _logRepository.GetLogsForGlobalPlugin(
                globalPluginId,
                request.Cursor
            ),
            _ => await _logRepository.GetAllLogs(request.Cursor),
        };
        var (paginatedLogs, lastLog) = await _paginationHelper.PaginateWithAuthAsync(
            logs,
            request.Limit
        );
        var nextCursor = lastLog == null ? null : new LogCursor(lastLog.TimeStamp, lastLog.Id);
        return (paginatedLogs, nextCursor);
    }
}
