using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Plugins;

/// <summary>
/// The repository for plugins that handles the data access.
/// </summary>
public class PluginRepository : RepositoryBase<Plugin>, IPluginRepository
{
    /// <summary>
    /// Constructor for the PluginRepository.
    /// </summary>
    /// <param name="context"></param>
    public PluginRepository(ProjectMetadataPlatformDbContext context)
        : base(context)
    {
        _context = context;
    }

    private readonly ProjectMetadataPlatformDbContext _context;

    /// <summary>
    /// Gets all plugins for a given project id from database.
    /// </summary>
    /// <param name="id">selects the project</param>
    /// <returns>The data received by the database.</returns>
    public async Task<IQueryable<ProjectPlugin>> GetAllPluginsForProjectIdAsync(int id)
    {
        if (!await _context.Projects.AnyAsync(p => p.Id == id))
        {
            throw new ProjectNotFoundException(id);
        }

        return _context
            .ProjectPluginsRelation.Where(rel => rel.ProjectId == id)
            .Include(rel => rel.Project)
            .Include(rel => rel.Plugin)
            .Include(rel => rel.PluginBilling);
    }

    /// <summary>
    /// Gets all unarchived plugins for a given project id from database.
    /// <param name="id">selects the project</param>
    /// <returns>The data received by the database.</returns>
    /// </summary>
    public async Task<IQueryable<ProjectPlugin>> GetAllUnarchivedPluginsForProjectIdAsync(int id)
    {
        if (!await _context.Projects.AnyAsync(p => p.Id == id))
        {
            throw new ProjectNotFoundException(id);
        }

        return _context
            .ProjectPluginsRelation.Where(rel =>
                rel.ProjectId == id && rel.Plugin != null && !rel.Plugin.IsArchived
            )
            .Include(rel => rel.Project)
            .Include(rel => rel.Plugin)
            .Include(rel => rel.PluginBilling);
    }

    /// <summary>
    /// Saves a given Plugin to the database.
    /// </summary>
    /// <param name="plugin">The Plugin to save</param>
    /// <returns>The saved Plugin</returns>
    public Task<Plugin> StorePlugin(Plugin plugin)
    {
        if (plugin.Id == 0)
        {
            _ = _context.Plugins.Add(plugin);
        }
        else
        {
            Update(plugin);
        }

        return Task.FromResult(plugin);
    }

    /// <summary>
    /// Saves a given Plugin to the database.
    /// </summary>
    /// <param name="plugin">The Plugin to save</param>
    /// <returns>The saved Plugin</returns>
    public async Task<ProjectPlugin> StoreProjectPlugin(ProjectPlugin plugin)
    {
        if (plugin.Id == 0)
        {
            var maxId =
                await _context
                    .ProjectPluginsRelation.Where(pp => pp.ProjectId == plugin.ProjectId)
                    .MaxAsync(pp => (int?)pp.Id)
                ?? 0;

            plugin.Id = ++maxId;
            _ = _context.ProjectPluginsRelation.Add(plugin);
        }
        else
        {
            _ = _context.ProjectPluginsRelation.Update(plugin);
        }

        return plugin;
    }

    /// <summary>
    /// Asynchronously retrieves a plugin by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the plugin to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Plugin that matches the provided id.</returns>
    public async Task<Plugin?> GetPluginByIdAsync(int id)
    {
        return await GetIf(p => p.Id == id).FirstOrDefaultAsync()
            ?? throw new PluginNotFoundException(id);
    }

    /// <inheritdoc />
    public async Task<ProjectPlugin> GetProjectPluginAsync(int projectId, int id)
    {
        return await _context
                .ProjectPluginsRelation.Include(p => p.Project)
                .Include(p => p.Plugin)
                .Where(p => p.Id == id && p.ProjectId == projectId)
                .FirstOrDefaultAsync()
            ?? throw new ProjectPluginNotFoundException(projectId, id);
    }

    /// <inheritdoc />
    public async Task<Plugin> GetGlobalPluginAsNoTrackingAsync(int id)
    {
        return await GetIf(p => p.Id == id).AsNoTracking().FirstOrDefaultAsync()
            ?? throw new PluginNotFoundException(id);
    }

    /// <summary>
    /// Gets all global plugins from the database.
    /// </summary>
    /// <returns>All global plugins</returns>
    public async Task<IQueryable<Plugin>> GetGlobalPluginsAsync()
    {
        return _context.Plugins;
    }

    /// <summary>
    /// Checks if a plugin exists.
    /// </summary>
    /// <returns>True, if the plugin with the given id exists</returns>
    public async Task<bool> CheckPluginExists(int id)
    {
        return await _context.Plugins.AnyAsync(plugin => plugin.Id == id);
    }

    /// <summary>
    /// Checks if a project plugin exists.
    /// </summary>
    /// <returns>True, if the plugin with the given project id, plugin id and url exists</returns>
    public async Task<bool> CheckProjectPluginExists(int projectId, int pluginId, string url)
    {
        return await _context.ProjectPluginsRelation.AnyAsync(plugin =>
            plugin.ProjectId == projectId && plugin.PluginId == pluginId && plugin.Url == url
        );
    }

    /// <summary>
    /// Deletes Global Plugin
    /// </summary>
    /// <param name="plugin"></param>
    /// <returns></returns>
    public Task<bool> DeleteGlobalPlugin(Plugin plugin)
    {
        _ = _context.Plugins.Remove(plugin);

        return Task.FromResult(true);
    }

    /// <summary>
    /// Deletes Project Plugin
    /// </summary>
    /// <param name="plugin"></param>
    /// <returns></returns>
    public Task<bool> DeleteProjectPlugin(ProjectPlugin plugin)
    {
        _ = _context.ProjectPluginsRelation.Remove(plugin);

        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public async Task<bool> CheckGlobalPluginNameExists(string name)
    {
        var lowerName = name.ToLower();
        var queryResult = GetIf(plugin => plugin.PluginName.ToLower().Equals(lowerName));
        return await queryResult.AnyAsync();
    }
}
