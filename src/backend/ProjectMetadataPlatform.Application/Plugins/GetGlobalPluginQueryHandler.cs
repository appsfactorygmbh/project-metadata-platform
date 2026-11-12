using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handler for <see cref="GetGlobalPluginQuery"/>
/// </summary>
public class GetGlobalPluginQueryHandler
    : IRequestHandler<GetGlobalPluginQuery, (Plugin, IEnumerable<AuthorizationConstants.Actions>)>
{
    private readonly IPluginRepository _pluginRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new Instance of <see cref="GetGlobalPluginQueryHandler"/>
    /// </summary>
    /// <param name="pluginRepository"></param>
    /// <param name="authorizationService"></param>
    public GetGlobalPluginQueryHandler(
        IPluginRepository pluginRepository,
        IAuthorizationService authorizationService
    )
    {
        _pluginRepository = pluginRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles Request to return plugin information by id.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    public async Task<(Plugin, IEnumerable<AuthorizationConstants.Actions>)> Handle(
        GetGlobalPluginQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var plugin = await _pluginRepository.GetPluginByIdAsync(request.Id);
        if (!await _authorizationService.CheckAccess(plugin, AuthorizationConstants.Actions.GET))
        {
            throw new UnauthorizedException();
        }
        var permissions = await _authorizationService.GetAllowedActions(
            plugin,
            [AuthorizationConstants.Actions.EDIT, AuthorizationConstants.Actions.DELETE]
        );
        return (plugin, permissions);
    }
}
