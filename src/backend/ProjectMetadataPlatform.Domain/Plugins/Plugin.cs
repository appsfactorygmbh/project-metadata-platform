using System.Collections.Generic;

namespace ProjectMetadataPlatform.Domain.Plugins;

/// <summary>
/// The representation of a plugin in the database.
/// </summary>
public class Plugin
{
    /// <summary>
    /// The id of the plugin.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// A boolean indicating if a plugin is archived/deleted.
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// The name of the plugin.
    /// </summary>
    public required string PluginName { get; set; }

    /// <summary>
    /// The base url of the plugin.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Holds the relation between Projects and Plugins.
    /// </summary>
    public ICollection<ProjectPlugins>? ProjectPlugins { get; set; }
}
