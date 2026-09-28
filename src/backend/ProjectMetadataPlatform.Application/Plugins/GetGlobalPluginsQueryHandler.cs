using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

///  <inheritdoc />
public class GetGlobalPluginsQueryHandler
    : IRequestHandler<
        GetGlobalPluginsQuery,
        (
            IEnumerable<(Plugin plugin, IEnumerable<AuthorizationConstants.Actions> permissions)>,
            IEnumerable<AuthorizationConstants.Actions>,
            PluginCursor?
        )
    >
{
    private readonly IPluginRepository _pluginRepository;
    private readonly IAuthorizationService _authorizationService;

    private readonly IPaginationHelper _paginationHelper;

    /// <summary>
    /// Creates a new instance of <see cref="GetGlobalPluginsQueryHandler"/>.
    /// </summary>
    public GetGlobalPluginsQueryHandler(
        IPluginRepository pluginRepository,
        IAuthorizationService authorizationService,
        IPaginationHelper paginationHelper
    )
    {
        _pluginRepository = pluginRepository;
        _authorizationService = authorizationService;
        _paginationHelper = paginationHelper;
    }

    /// <inheritdoc />
    public async Task<(
        IEnumerable<(Plugin plugin, IEnumerable<AuthorizationConstants.Actions> permissions)>,
        IEnumerable<AuthorizationConstants.Actions>,
        PluginCursor?
    )> Handle(GetGlobalPluginsQuery request, CancellationToken cancellationToken)
    {
        var pluginQuery = await _pluginRepository.GetGlobalPluginsAsync(request.Cursor);
        var (paginatedPlugins, lastPlugin) = await _paginationHelper.PaginateWithAuthAsync(
            pluginQuery,
            request.Limit
        );

        var globalPermissions = await _authorizationService.GetAllowedActions<Plugin>(
            actions: [AuthorizationConstants.Actions.CREATE]
        );
        List<(Plugin, IEnumerable<AuthorizationConstants.Actions>)> plugins = [];
        foreach (var plugin in paginatedPlugins)
        {
            plugins.Add(
                (
                    plugin,
                    await _authorizationService.GetAllowedActions(
                        plugin,
                        [AuthorizationConstants.Actions.EDIT, AuthorizationConstants.Actions.DELETE]
                    )
                )
            );
        }
        var nextCursor = lastPlugin == null ? null : new PluginCursor(lastPlugin.PluginName);
        return (plugins, globalPermissions, nextCursor);
    }
}
