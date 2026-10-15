using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Query to get all global plugins.
/// </summary>
public record GetGlobalPluginsQuery(PluginCursor? Cursor, int? Limit)
    : IRequest<(
        IEnumerable<(Plugin plugin, IEnumerable<AuthorizationConstants.Actions> permissions)>,
        IEnumerable<AuthorizationConstants.Actions>,
        PluginCursor?
    )>;
