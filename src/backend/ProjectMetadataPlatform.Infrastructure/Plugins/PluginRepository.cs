using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public async Task<List<ProjectPlugins>> GetAllPluginsForProjectIdAsync(int id)
    {
        if (!await _context.Projects.AnyAsync(p => p.Id == id))
        {
            throw new ProjectNotFoundException(id);
        }

        return await _context
            .ProjectPluginsRelation.Where(rel => rel.ProjectId == id)
            .Include(rel => rel.Plugin)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all unarchived plugins for a given project id from database.
    /// <param name="id">selects the project</param>
    /// <returns>The data received by the database.</returns>
    /// </summary>
    public async Task<List<ProjectPlugins>> GetAllUnarchivedPluginsForProjectIdAsync(int id)
    {
        if (!await _context.Projects.AnyAsync(p => p.Id == id))
        {
            throw new ProjectNotFoundException(id);
        }

        return await _context
            .ProjectPluginsRelation.Where(rel =>
                rel.ProjectId == id && rel.Plugin != null && !rel.Plugin.IsArchived
            )
            .Include(rel => rel.Plugin)
            .ToListAsync();
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
    /// Asynchronously retrieves a plugin by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the plugin to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Plugin that matches the provided id.</returns>
    public async Task<Plugin?> GetPluginByIdAsync(int id)
    {
        return await GetIf(p => p.Id == id).FirstOrDefaultAsync()
            ?? throw new PluginNotFoundException(id);
    }

    /// <summary>
    /// Gets all global plugins from the database.
    /// </summary>
    /// <returns>All global plugins</returns>
    public async Task<IEnumerable<Plugin>> GetGlobalPluginsAsync()
    {
        return await _context.Plugins.ToListAsync();
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
    /// Deletes Global Plugin
    /// </summary>
    /// <param name="plugin"></param>
    /// <returns></returns>
    public Task<bool> DeleteGlobalPlugin(Plugin plugin)
    {
        _context.Plugins.Remove(plugin);

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
