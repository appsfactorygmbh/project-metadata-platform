namespace ProjectMetadataPlatform.Api.Plugins.Models;

/// <summary>
/// Response Model representing a Plugin.
/// </summary>
/// <param name="PluginName">Name of the plugin</param>
/// <param name="Id">Id of the plugin</param>
/// <param name="IsArchived">If the plugin is archived or not</param>
/// <param name="Keys">empty array keys</param>
/// <param name="BaseUrl">Base URL of the plugin</param>
public record GetGlobalPluginResponse(
    string PluginName,
    int Id,
    bool IsArchived,
    string[] Keys,
    string? BaseUrl
);
