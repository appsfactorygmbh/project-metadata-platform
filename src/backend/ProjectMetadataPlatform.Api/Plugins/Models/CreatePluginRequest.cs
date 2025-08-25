using System.Collections.Generic;

namespace ProjectMetadataPlatform.Api.Plugins.Models;

/// <summary>
/// Request for creating a new plugin.
/// </summary>
/// <param name="PluginName">The name of the new plugin.</param>
/// <param name="IsArchived">A boolean indicating if a plugin is archived/deleted.</param>
/// <param name="Keys">The keys of the new plugin.</param>
/// <param name="BaseUrl">Base Url of the new plugin.</param>
public record CreatePluginRequest(
    string PluginName,
    bool IsArchived,
    List<string> Keys,
    string BaseUrl
);
