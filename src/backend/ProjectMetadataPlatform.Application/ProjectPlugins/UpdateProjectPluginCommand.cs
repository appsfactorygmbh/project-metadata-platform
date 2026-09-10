using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.ProjectPlugins;

/// <summary>
/// Request to update a project plugin.
/// </summary>
/// <param name="ProjectId">Id of the Project.</param>
/// <param name="ProjectpluginId">Id of the plugin.</param>
/// <param name="Name">Name of the plugin.</param>
/// <param name="Url">Url of the plugin.</param>
public record UpdateProjectPluginCommand(
    int ProjectId,
    int ProjectpluginId,
    string Name,
    string Url
) : IRequest<ProjectPlugin>;
