using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

/// <summary>
/// Exception for when a plugin is not archived when deleting it.
/// </summary>
/// <param name="plugin">The plugin that could not be deleted.</param>
public class PluginNotArchivedException(Plugin plugin)
    : PluginException("The plugin " + plugin.Id + " is not archived.");
