using System.Collections.Generic;
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
    Task<List<ProjectPlugins>> GetAllPluginsForProjectIdAsync(int id);

    /// <summary>
    /// Returns a collection of all unarchived plugins for a given project id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<List<ProjectPlugins>> GetAllUnarchivedPluginsForProjectIdAsync(int id);

    /// <summary>
    /// Saves a given Plugin to the database.
    /// </summary>
    /// <param name="plugin">The Plugin to save</param>
    /// <returns></returns>
    Task<Plugin> StorePlugin(Plugin plugin);

    /// <summary>
    /// Gets a specific Plugin by its id.
    /// </summary>
    /// <param name="id">The id of the plugin</param>
    /// <returns></returns>
    Task<Plugin?> GetPluginByIdAsync(int id);

    /// <summary>
    /// Returns all global plugins
    /// </summary>
    /// <returns>Collection of all global plugins</returns>
    Task<IEnumerable<Plugin>> GetGlobalPluginsAsync();

    /// <summary>
    /// Checks if a plugin exists.
    /// </summary>
    /// <returns>True, if the plugin with the given id exists</returns>
    Task<bool> CheckPluginExists(int id);

    /// <summary>
    /// Deletes global Plugin
    /// </summary>
    /// <param name="plugin"></param>
    /// <returns>True, if the plugin is deleted in the database</returns>
    Task<bool> DeleteGlobalPlugin(Plugin plugin);

    /// <summary>
    /// Checks if a global plugin with the given name exists.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>A boolean indicating whether a plugin with the given name exists.</returns>
    Task<bool> CheckGlobalPluginNameExists(string name);
}
