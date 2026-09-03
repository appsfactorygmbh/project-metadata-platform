using ProjectMetadataPlatform.Domain.Billing;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Domain.Plugins;

/// <summary>
/// The representation of a relation between a Project and a Plugin in the Database.
/// </summary>
public class ProjectPlugin
{
    /// <summary>
    /// Project specific Id of the plugin.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The project stored in the relation.
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    /// The plugin stored in the relation.
    /// </summary>
    public Plugin? Plugin { get; set; }

    /// <summary>
    /// The id for a plugin.
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

    /// <summary>
    /// Id of plugin specific billing information.
    /// </summary>
    public int? BillingId { get; set; }

    /// <summary>
    /// The billing information stored in the relation.
    /// </summary>
    public PluginBilling? PluginBilling { get; set; }
}
