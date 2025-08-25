using System.Collections.Generic;
using MediatR;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Command to create a new Plugin with the given name.
/// </summary>
/// <param name="Name">The name of the new plugin</param>
/// <param name="IsArchived">A boolean indicating if a plugin is archived/deleted.</param>
/// <param name="Keys">The keys of the new plugin.</param>
/// <param name="BaseUrl">The Base Url of the new plugin.</param>
public record CreatePluginCommand(string Name, bool IsArchived, List<string> Keys, string BaseUrl)
    : IRequest<int>;
