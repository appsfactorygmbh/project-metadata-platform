using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

/// <summary>
/// Exception thrown when a project plugin is not found.
/// </summary>
/// <param name="projectId">Id of the project that was searched for.</param>
/// <param name="pluginId">Id of the plugin that was searched for.</param>
public class ProjectPluginNotFoundException(int projectId, int pluginId)
    : EntityNotFoundException(
        "The plugin with id "
            + pluginId
            + " was not found in the project with the id "
            + projectId
            + "."
    ) { }
