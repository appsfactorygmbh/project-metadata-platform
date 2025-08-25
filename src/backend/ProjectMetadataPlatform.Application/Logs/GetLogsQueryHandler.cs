using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Logs;

/// <summary>
/// Handles the query to retrieve logs based on project ID and search criteria.
/// </summary>
public class GetLogsQueryHandler : IRequestHandler<GetLogsQuery, IEnumerable<Log>>
{
    private readonly ILogRepository _logRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetLogsQueryHandler"/> class.
    /// </summary>
    /// <param name="logRepository">The log repository instance.</param>
    public GetLogsQueryHandler(ILogRepository logRepository)
    {
        _logRepository = logRepository;
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
    public async Task<IEnumerable<Log>> Handle(
        GetLogsQuery request,
        CancellationToken cancellationToken
    )
    {
        return request switch
        {
            { ProjectId: { } projectId } => await _logRepository.GetLogsForProject(projectId),
            { Search: { } search } => await _logRepository.GetLogsWithSearch(search),
            { UserId: { } userId } => await _logRepository.GetLogsForUser(userId),
            { GlobalPluginId: { } globalPluginId } => await _logRepository.GetLogsForGlobalPlugin(
                globalPluginId
            ),
            _ => await _logRepository.GetAllLogs(),
        };
    }
}
