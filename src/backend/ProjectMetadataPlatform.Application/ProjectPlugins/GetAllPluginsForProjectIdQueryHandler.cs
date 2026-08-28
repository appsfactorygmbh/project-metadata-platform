using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.ProjectPlugins.Models;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.ProjectPlugins;

/// <summary>
/// Handler for the <see cref="GetAllPluginsForProjectIdQuery" />
/// </summary>
public class GetAllPluginsForProjectIdQueryHandler
    : IRequestHandler<
        GetAllPluginsForProjectIdQuery,
        (IEnumerable<ProjectPluginPermissionModel>, IEnumerable<AuthorizationConstants.Actions>)
    >
{
    private readonly IPluginRepository _pluginRepository;

    private readonly IAuthorizationService _authorizationService;
    private readonly IBillingRepository _billingRepository;

    /// <summary>
    /// Creates a new instance of<see cref="GetAllPluginsForProjectIdQueryHandler" />.
    /// </summary>
    /// <param name="pluginRepository"></param>
    /// <param name="authorizationService"></param>
    /// <param name="billingRepository"></param>
    public GetAllPluginsForProjectIdQueryHandler(
        IPluginRepository pluginRepository,
        IAuthorizationService authorizationService,
        IBillingRepository billingRepository
    )
    {
        _pluginRepository = pluginRepository;
        _authorizationService = authorizationService;
        ;
        _billingRepository = billingRepository;
    }

    /// <summary>
    /// Handles the request to get all plugins for a given project id.
    /// </summary>
    /// <param name="request">the request that needs to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>the response of the request</returns>
    public async Task<(
        IEnumerable<ProjectPluginPermissionModel>,
        IEnumerable<AuthorizationConstants.Actions>
    )> Handle(GetAllPluginsForProjectIdQuery request, CancellationToken cancellationToken)
    {
        var pluginQuery = await _pluginRepository.GetAllPluginsForProjectIdAsync(request.Id);
        var queriedPlugins = await _authorizationService.TryGetPlanResourceQuery(pluginQuery);
        var globalPermissions = await _authorizationService.GetAllowedActions<Plugin>(
            actions: [AuthorizationConstants.Actions.CREATE]
        );
        List<ProjectPluginPermissionModel> plugins = [];
        if (queriedPlugins == null)
        {
            var pluginList = await pluginQuery.ToListAsync(cancellationToken);

            foreach (var plugin in pluginList)
            {
                if (
                    await _authorizationService.CheckAccess(
                        plugin,
                        AuthorizationConstants.Actions.GET
                    )
                )
                {
                    plugins.Add(await BuildPPPM(plugin));
                }
            }
            return (plugins, globalPermissions);
        }

var queriedPluginList = await queriedPlugins.ToListAsync(cancellationToken);

    foreach (var plugin in queriedPluginList){
            plugins.Add(await BuildPPPM(plugin));
        }
        return (plugins, globalPermissions);
    }

    private async Task<ProjectPluginPermissionModel> BuildPPPM(ProjectPlugin plugin)
    {
        var pluginPermissions = await _authorizationService.GetAllowedActions(
            plugin,
            [AuthorizationConstants.Actions.EDIT, AuthorizationConstants.Actions.DELETE]
        );

        var billing =
            plugin.PluginBilling == null
                ? null
                : await _billingRepository.GetPluginBillingByIdAsync(plugin.ProjectId, plugin.Id);

        var billingPermissions = await _authorizationService.GetAllowedActions(
            billing,
            billing == null
                ? [AuthorizationConstants.Actions.CREATE]
                : [AuthorizationConstants.Actions.GET]
        );

        return new ProjectPluginPermissionModel(plugin, pluginPermissions, billingPermissions);
    }
}
