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
    : IRequestHandler<GetGlobalPluginsQuery, IEnumerable<Plugin>>
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
    public async Task<IEnumerable<Plugin>> Handle(
        GetGlobalPluginsQuery request,
        CancellationToken cancellationToken
    )
    {
        var plugins = await _pluginRepository.GetGlobalPluginsAsync();
        var queriedPlugins = await _authorizationService.TryGetPlanResourceQuery(plugins);
        if (queriedPlugins == null)
        {
            List<Plugin> filteredPlugins = [];
            foreach (var plugin in plugins)
            {
                if (
                    (
                        await _authorizationService.CheckAccess(
                            plugin,
                            [AuthorizationConstants.Actions.GET]
                        )
                    )[AuthorizationConstants.Actions.GET]
                )
                {
                    filteredPlugins.Add(plugin);
                }
            }
            return filteredPlugins;
        }
        return await queriedPlugins.ToListAsync(cancellationToken: cancellationToken);
    }
}
