using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

/// <summary>
/// Error thrown when a project plugin with a specific url already exists on a project.
/// </summary>
/// <param name="projectId"> Id of the project.</param>
/// <param name="pluginId">Id of the global plugin.</param>
/// <param name="url">Url of the project plugin.</param>
public class ProjectPluginAlreadyExistsException(int projectId, int pluginId, string url)
    : EntityAlreadyExistsException(
        "A project Plugin with the url "
            + url
            + " with a global plugin with the id "
            + pluginId
            + " already exists on the project with the id "
            + projectId
            + "."
    );
