namespace ProjectMetadataPlatform.Api.ProjectPlugins.Models;

/// <summary>
/// Response model representing a Plugin of a project.
/// </summary>
/// <param name="Url">The URL of this plugin instance in the project.</param>
/// <param name="DisplayName">The name of this plugin instance in the project.</param>
public record UpdateProjectPluginRequest(string Url, string DisplayName);
