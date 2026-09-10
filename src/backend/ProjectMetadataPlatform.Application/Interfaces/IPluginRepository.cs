using System.Linq;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for plugins
/// </summary>
public interface IPluginRepository
{
    /// <summary>
    /// Returns a collection of plugins for a given project id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IQueryable<ProjectPlugin>> GetAllPluginsForProjectIdAsync(int id);

    /// <summary>
    /// Returns a collection of all unarchived plugins for a given project id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IQueryable<ProjectPlugin>> GetAllUnarchivedPluginsForProjectIdAsync(int id);

    /// <summary>
    /// Saves a given Plugin to the database.
    /// </summary>
    /// <param name="plugin">The Plugin to save</param>
    /// <returns></returns>
    Task<Plugin> StorePlugin(Plugin plugin);

    /// <summary>
    /// Saves a given Project Plugin to the database.
    /// </summary>
    /// <param name="plugin">The Plugin to save</param>
    /// <returns></returns>
    Task<ProjectPlugin> StoreProjectPlugin(ProjectPlugin plugin);

    /// <summary>
    /// Gets a specific Plugin by its id.
    /// </summary>
    /// <param name="id">The id of the plugin</param>
    /// <returns></returns>
    Task<Plugin?> GetPluginByIdAsync(int id);

    /// <summary>
    /// Gets a specific Project Plugin by its ids.
    /// </summary>
    /// <param name="projectId">The Id of the Project</param>
    /// <param name="id">The id of the plugin</param>
    /// <returns></returns>
    Task<ProjectPlugin> GetProjectPluginAsync(int projectId, int id);

    /// <summary>
    /// Returns a global plugin with the specific id without tracking entity changes.
    /// </summary>
    /// <param name="id">Id of the plugin.</param>
    /// <returns>Global Plugin if it exists.</returns>
    Task<Plugin> GetGlobalPluginAsNoTrackingAsync(int id);

    /// <summary>
    /// Returns all global plugins
    /// </summary>
    /// <returns>Collection of all global plugins</returns>
    Task<IQueryable<Plugin>> GetGlobalPluginsAsync();

    /// <summary>
    /// Checks if a plugin exists.
    /// </summary>
    /// <returns>True, if the plugin with the given id exists</returns>
    Task<bool> CheckPluginExists(int id);

    /// <summary>
    /// Checks if a Project plugin from a global plugin with a specific url exists on a project
    /// </summary>
    /// <param name="projectId">Project Id</param>
    /// <param name="pluginId">Global Plugin Id</param>
    /// <param name="url">Url string</param>
    /// <returns>True if the project plugin exists</returns>
    Task<bool> CheckProjectPluginExists(int projectId, int pluginId, string url);

    /// <summary>
    /// Deletes global Plugin
    /// </summary>
    /// <param name="plugin"></param>
    /// <returns>True, if the plugin is deleted in the database</returns>
    Task<bool> DeleteGlobalPlugin(Plugin plugin);

    /// <summary>
    /// Deletes project Plugin
    /// </summary>
    /// <param name="plugin"></param>
    /// <returns>True, if the plugin is deleted in the database</returns>
    Task<bool> DeleteProjectPlugin(ProjectPlugin plugin);

    /// <summary>
    /// Checks if a global plugin with the given name exists.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>A boolean indicating whether a plugin with the given name exists.</returns>
    Task<bool> CheckGlobalPluginNameExists(string name);
}
