using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

/// <summary>
/// Exception thrown when a plugin is not found.
/// </summary>
/// <param name="pluginId">Id of the plugin that was searched for.</param>
public class PluginNotFoundException(int pluginId)
    : EntityNotFoundException("The plugin with id " + pluginId + " was not found.") { }
