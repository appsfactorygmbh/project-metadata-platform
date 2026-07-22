using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
            IEnumerable<AuthorizationConstants.Actions>
        )
    >
{
    private readonly IPluginRepository _pluginRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetGlobalPluginsQueryHandler"/>.
    /// </summary>
    public GetGlobalPluginsQueryHandler(
        IPluginRepository pluginRepository,
        IAuthorizationService authorizationService
    )
    {
        _pluginRepository = pluginRepository;
        _authorizationService = authorizationService;
    }

    /// <inheritdoc />
    public async Task<(
        IEnumerable<(Plugin plugin, IEnumerable<AuthorizationConstants.Actions> permissions)>,
        IEnumerable<AuthorizationConstants.Actions>
    )> Handle(GetGlobalPluginsQuery request, CancellationToken cancellationToken)
    {
        var pluginQuery = await _pluginRepository.GetGlobalPluginsAsync();
        var queriedPlugins = await _authorizationService.TryGetPlanResourceQuery(pluginQuery);

        var globalPermissions = await _authorizationService.GetPermissions<Plugin>(actions:[AuthorizationConstants.Actions.CREATE]);
        List<(Plugin, IEnumerable<AuthorizationConstants.Actions>)> plugins = [];
        if (queriedPlugins == null)
        {
            foreach (var plugin in pluginQuery)
            {
                if (
                    await _authorizationService.CheckAccess(
                        plugin,
                        AuthorizationConstants.Actions.GET
                    )
                )
                {
                    plugins.Add((plugin, await _authorizationService.GetPermissions(plugin,[AuthorizationConstants.Actions.EDIT,AuthorizationConstants.Actions.DELETE])));
                }
            }
            return (plugins, globalPermissions);
        }

        foreach (
            var plugin in await queriedPlugins.ToListAsync(cancellationToken: cancellationToken)
        )
        {
            plugins.Add((plugin, await _authorizationService.GetPermissions(plugin,[AuthorizationConstants.Actions.EDIT,AuthorizationConstants.Actions.DELETE])));
        }
        return (plugins, globalPermissions);
    }
}
