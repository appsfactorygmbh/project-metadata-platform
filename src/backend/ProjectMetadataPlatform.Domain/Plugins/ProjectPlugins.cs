using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Domain.Plugins;

/// <summary>
/// The representation of a relation between a Project and a Plugin in the Database.
/// </summary>
public class ProjectPlugins
{
    /// <summary>
    /// The project stored in the relation.
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    /// The plugin stored in the relation.
    /// </summary>
    public Plugin? Plugin { get; set; }

    /// <summary>
    /// The id for a plugin used as a foreign key for the plugin.
    /// </summary>
    public required int PluginId { get; set; }

    /// <summary>
    /// The id for a project used as a foreign key for the project.
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// The display name for the plugin.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Url for the plugin.
    /// </summary>
    public required string Url { get; set; }
}
