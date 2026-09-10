using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.ProjectPlugins;

/// <summary>
/// Request to delete a project plugin.
/// </summary>
/// <param name="ProjectId">Id of the project.</param>
/// <param name="ProjectPluginId">Id of the plugin.</param>
public record DeleteProjectPluginCommand(int ProjectId, int ProjectPluginId) : IRequest;
