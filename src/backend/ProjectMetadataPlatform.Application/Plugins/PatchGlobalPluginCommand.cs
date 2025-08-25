using MediatR;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Represents a command to update a global plugin.
/// </summary>
/// <param name="Id">The unique identifier of the plugin to be patched.</param>
/// <param name="PluginName">The new name of the plugin. Null if not being updated.</param>
/// <param name="IsArchived">The new archived status of the plugin. Null if not being updated.</param>
/// <param name="BaseUrl">The plugin's new base url.</param>
/// <returns>A Plugin object that represents the updated plugin.</returns>
public record PatchGlobalPluginCommand(
    int Id,
    string? PluginName = null,
    bool? IsArchived = null,
    string? BaseUrl = null
) : IRequest<Plugin>;
