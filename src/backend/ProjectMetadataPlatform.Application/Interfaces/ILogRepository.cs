using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
///     Repository for logging project changes
/// </summary>
public interface ILogRepository
{
    /// <summary>
    /// Adds Logs for changes made to a project. Sets the current User as the Author.
    /// </summary>
    /// <param name="project">The Project changes were made to.</param>
    /// <param name="action">The type of change that was made.</param>
    /// <param name="changes">A list of the changed properties.</param>
    /// <returns></returns>
    Task AddProjectLogForCurrentUser(Project project, Action action, List<LogChange> changes);

    /// <summary>
    /// Adds Logs for changes made to a User. Sets the current User as the Author.
    /// </summary>
    /// <param name="affectedUser">The User changes were made to.</param>
    /// <param name="action">The type of change that was made.</param>
    /// <param name="changes">A list of the changed properties.</param>
    /// <returns></returns>
    Task AddUserLogForCurrentUser(
        IdentityUser affectedUser,
        Action action,
        List<LogChange> changes
    );

    /// <summary>
    /// Adds Logs for changes made to a GlobalPlugin. Sets the current User as the Author.
    /// </summary>
    /// <param name="globalPlugin">The GlobalPlugin changes were made to.</param>
    /// <param name="action">The type of change that was made.</param>
    /// <param name="changes">A list of the changed properties.</param>
    /// <returns></returns>
    Task AddGlobalPluginLogForCurrentUser(
        Plugin globalPlugin,
        Action action,
        List<LogChange> changes
    );

    /// <summary>
    /// Adds Logs for changes made to a team. Sets the current User as the Author.
    /// </summary>
    /// <param name="team">The team changes were made to.</param>
    /// <param name="action">The type of change that was made.</param>
    /// <param name="changes">A list of the changed properties.</param>
    /// <returns></returns>
    Task AddTeamLogForCurrentUser(Team team, Action action, List<LogChange> changes);

    /// <summary>
    /// Retrieves the logs for a specific project.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <returns>A list of logs associated with the specified project.</returns>
    Task<List<Log>> GetLogsForProject(int projectId);

    /// <summary>
    /// Retrieves logs that match the specified search term.
    /// </summary>
    /// <param name="search">The search term to filter logs.</param>
    /// <returns>A list of logs that match the search term.</returns>
    Task<List<Log>> GetLogsWithSearch(string search);

    /// <summary>
    /// Retrieves the logs for a specific project.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns> A list of logs associated with the specified user.</returns>
    Task<List<Log>> GetLogsForUser(string userId);

    /// <summary>
    /// Retrieves the logs for a specific project.
    /// </summary>
    /// <param name="globalPluginId">The unique identifier of the global plugin.</param>
    /// <returns> A list of logs associated with the specified global plugin.</returns>
    Task<List<Log>> GetLogsForGlobalPlugin(int globalPluginId);

    /// <summary>
    /// Retrieves all logs from the database.
    /// </summary>
    /// <returns>A list of all logs, sorted by timestamp.</returns>
    Task<List<Log>> GetAllLogs();
}
