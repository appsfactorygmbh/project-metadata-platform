namespace ProjectMetadataPlatform.Api.ProjectPlugins.Models;

/// <summary>
/// Request for adding a Plugin to a Project
/// </summary>
/// <param name="Url">The URL of this plugin instance in the project.</param>
/// <param name="DisplayName">The name of this plugin instance in the project.</param>
/// <param name="PluginId">The global id of the plugin instance in the project.</param>
public record AddProjectPluginRequest(string Url, string DisplayName, int PluginId);
