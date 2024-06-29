namespace ProjectMetadataPlatform.Api.Plugins.Models;

/// <summary>
/// Represents a request to patch a global plugin.
/// </summary>
/// <param name="PluginName">The name of the plugin. Null if not being updated.</param>
/// <param name="IsArchived">The archived status of the plugin. Null if not being updated.</param>
public record PatchGlobalPluginRequest(string? PluginName = null, bool? IsArchived = null);
