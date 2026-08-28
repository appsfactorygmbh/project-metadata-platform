using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.BillingExceptions;

/// <summary>
/// Thrown if no billing information is found for a plugin of a project.
/// </summary>
/// <param name="projectId">Id of the project.</param>
/// <param name="pluginId">Id of the plugin.</param>
public class PluginBillingInformationNotFoundException(int projectId, int pluginId)
    : EntityNotFoundException(
        "No billing information for the plugin with the id "
            + pluginId
            + " of the project with the id "
            + projectId
            + " was found."
    ) { }
