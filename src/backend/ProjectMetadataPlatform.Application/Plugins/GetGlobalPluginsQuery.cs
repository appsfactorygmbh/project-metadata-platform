using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Query to get all global plugins.
/// </summary>
/// <param name="IsArchived"> Wether to filter by the plugins archived status. </param>
/// <param name="Cursor">Optional Cursor for Pagination.</param>
/// <param name="Limit">Optional Limit for Pagination. </param>
public record GetGlobalPluginsQuery(bool? IsArchived, PluginCursor? Cursor, int? Limit)
    : IRequest<(
        IEnumerable<(Plugin plugin, IEnumerable<AuthorizationConstants.Actions> permissions)>,
        IEnumerable<AuthorizationConstants.Actions>,
        PluginCursor?
    )>;
