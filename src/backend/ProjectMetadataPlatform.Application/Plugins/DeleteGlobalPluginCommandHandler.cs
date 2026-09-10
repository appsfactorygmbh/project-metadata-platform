using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handler for the <see cref="DeleteGlobalPluginCommand"/>
/// </summary>
public class DeleteGlobalPluginCommandHandler : IRequestHandler<DeleteGlobalPluginCommand, bool>
{
    private readonly IPluginRepository _pluginRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of<see cref="DeleteGlobalPluginCommandHandler"/>.
    /// </summary>
    /// <param name="pluginRepository"></param>
    /// <param name="logRepository"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="authorizationService"></param>
    public DeleteGlobalPluginCommandHandler(
        IPluginRepository pluginRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService
    )
    {
        _pluginRepository = pluginRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Delete Plugin with the given id.
    /// </summary>
    /// <param name="request">The request that needs to be handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The response of the request.</returns>
    public async Task<bool> Handle(
        DeleteGlobalPluginCommand request,
        CancellationToken cancellationToken
    )
    {
        var plugin =
            await _pluginRepository.GetPluginByIdAsync(request.Id)
            ?? throw new PluginNotFoundException(request.Id);

        if (!await _authorizationService.CheckAccess(plugin, AuthorizationConstants.Actions.DELETE))
        {
            throw new UnauthorizedException();
        }
        if (plugin.IsArchived)
        {
            _ = await _pluginRepository.DeleteGlobalPlugin(plugin);

            var changes = new List<LogChange>();

            await _logRepository.AddGlobalPluginLogForCurrentActor(
                plugin,
                Action.REMOVED_GLOBAL_PLUGIN,
                changes
            );
            foreach (var projectPlugin in plugin.ProjectPlugins ?? [])
            {
                var removedPluginChanges = new List<LogChange>()
                {
                    new()
                    {
                        Property = nameof(ProjectPlugin.Plugin),
                        OldValue = plugin.PluginName,
                        NewValue = string.Empty,
                    },
                    new()
                    {
                        Property = nameof(ProjectPlugin.DisplayName),
                        OldValue = projectPlugin.DisplayName ?? string.Empty,
                        NewValue = string.Empty,
                    },
                    new()
                    {
                        Property = nameof(ProjectPlugin.Url),
                        OldValue = projectPlugin.Url,
                        NewValue = string.Empty,
                    },
                };

                await _logRepository.AddProjectLogForCurrentActor(
                    projectPlugin.Project!,
                    projectPlugin.PluginBilling == null
                        ? Action.REMOVED_PROJECT_PLUGIN
                        : Action.REMOVED_PROJECT_PLUGIN_WITH_BILLING,
                    removedPluginChanges,
                    globalPlugin: plugin
                );
            }
            await _unitOfWork.CompleteAsync();
            return true;
        }

        throw new PluginNotArchivedException(plugin);
    }
}
