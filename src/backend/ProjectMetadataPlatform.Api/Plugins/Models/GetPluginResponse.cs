namespace ProjectMetadataPlatform.Api.Plugins.Models;

/// <summary>
/// Response model representing a Plugin of a project.
/// </summary>
/// <param name="PluginName">The name of the plugin.</param>
/// <param name="Url">The URL of this plugin instance in the project.</param>
/// <param name="DisplayName">The name of this plugin instance in the project.</param>
/// <param name="Id">The global id of the plugin instance in the project.</param>
public record GetPluginResponse(string PluginName, string Url, string DisplayName, int Id);
