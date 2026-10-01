using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Methods for manually triggering BackgroundTasks
/// </summary>
public interface IJobScheduler
{
    /// <summary>
    /// Manually trigger RefreshSearchViewJob to run in 5 seconds.
    /// </summary>
    /// <returns></returns>
    Task ScheduleSearchViewRefreshAsync();
}
