using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.ProjectPlugins;

/// <summary>
/// Request to create a new project plugin.
/// </summary>
/// <param name="ProjectId">Id of the project.</param>
/// <param name="PluginId">Id of the global plugin.</param>
/// <param name="Name">Displayname of the plugin.</param>
/// <param name="Url">Url of the plugin.</param>
public record AddProjectPluginCommand(int ProjectId, int PluginId, string Name, string Url)
    : IRequest<int>;
