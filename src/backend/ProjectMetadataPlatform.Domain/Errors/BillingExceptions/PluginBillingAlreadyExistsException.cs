using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.BillingExceptions;

/// <summary>
/// Thrown if plugin billing already exists on a project plugin.
/// </summary>
/// <param name="pluginId">Id of the plugin.</param>
public class PluginBillingAlreadyExistsException(int pluginId)
    : EntityAlreadyExistsException(
        "Billing information already exists for the plugin with the id " + pluginId + "."
    );
