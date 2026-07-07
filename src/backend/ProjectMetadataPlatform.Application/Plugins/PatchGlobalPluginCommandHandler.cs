using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handles the PatchGlobalPluginCommand request.
/// </summary>
public class PatchGlobalPluginCommandHandler : IRequestHandler<PatchGlobalPluginCommand, Plugin>
{
    private readonly IPluginRepository _pluginRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatchGlobalPluginCommandHandler"/> class.
    /// </summary>
    /// <param name="pluginRepository">The plugin repository to use for plugin operations.</param>
    /// <param name="logRepository">The log repository to use for logging operations.</param>
    /// <param name="unitOfWork">The unit of work to use for transactional operations.</param>
    /// <param name="authorizationService"></param>
    public PatchGlobalPluginCommandHandler(
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
    /// Handles the PatchGlobalPluginCommand request.
    /// </summary>
    /// <param name="request">The PatchGlobalPluginCommand request to handle.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Plugin that was updated.</returns>
    /// <exception cref="PluginNameAlreadyExistsException">The Plugin name already exists.</exception>
    public async Task<Plugin> Handle(
        PatchGlobalPluginCommand request,
        CancellationToken cancellationToken
    )
    {
        var plugin =
            await _pluginRepository.GetPluginByIdAsync(request.Id)
            ?? throw new PluginNotFoundException(request.Id);
        await CheckAuthorization(plugin, request);
        if (
            request.PluginName != null
            && !string.Equals(
                plugin.PluginName,
                request.PluginName,
                StringComparison.OrdinalIgnoreCase
            )
            && await _pluginRepository.CheckGlobalPluginNameExists(request.PluginName)
        )
        {
            throw new PluginNameAlreadyExistsException(request.PluginName);
        }

        await AddUpdatedPluginLog(plugin, request);

        if (request.PluginName != null)
        {
            plugin.PluginName = request.PluginName;
        }

        if (
            request.BaseUrl != null
            && !string.Equals(plugin.BaseUrl, request.BaseUrl, StringComparison.Ordinal)
        )
        {
            plugin.BaseUrl = request.BaseUrl;
        }

        if (request.IsArchived != null && plugin.IsArchived != request.IsArchived.Value)
        {
            plugin.IsArchived = request.IsArchived.Value;
            await _logRepository.AddGlobalPluginLogForCurrentActor(
                plugin,
                plugin.IsArchived ? Action.ARCHIVED_GLOBAL_PLUGIN : Action.UNARCHIVED_GLOBAL_PLUGIN,
                []
            );
        }

        var updatedPlugin = await _pluginRepository.StorePlugin(plugin);
        await _unitOfWork.CompleteAsync();

        return updatedPlugin;
    }

    private async Task AddUpdatedPluginLog(Plugin plugin, PatchGlobalPluginCommand request)
    {
        var changes = new List<LogChange>();

        if (
            request.PluginName != null
            && !string.Equals(plugin.PluginName, request.PluginName, StringComparison.Ordinal)
        )
        {
            changes.Add(
                new LogChange
                {
                    Property = nameof(plugin.PluginName),
                    OldValue = plugin.PluginName,
                    NewValue = request.PluginName,
                }
            );
        }

        if (
            request.BaseUrl != null
            && !string.Equals(plugin.BaseUrl, request.BaseUrl, StringComparison.Ordinal)
        )
        {
            changes.Add(
                new LogChange
                {
                    Property = nameof(plugin.BaseUrl),
                    OldValue = plugin.BaseUrl ?? "",
                    NewValue = request.BaseUrl,
                }
            );
        }

        if (changes.Count > 0)
        {
            await _logRepository.AddGlobalPluginLogForCurrentActor(
                plugin,
                Action.UPDATED_GLOBAL_PLUGIN,
                changes
            );
        }
    }

    /// <summary>
    /// Checks Authorization for a Global Plugin and its update request.
    /// </summary>
    /// <param name="plugin">Requested Plugin</param>
    /// <param name="request">Update Request for the Plugin</param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException">Thrown if Update Request is unauthorized</exception>
    private async Task CheckAuthorization(Plugin plugin, PatchGlobalPluginCommand request)
    {
        Dictionary<string, object?> updates = [];
        if (request.PluginName != plugin.PluginName)
        {
            updates.Add(nameof(Plugin.PluginName), request.PluginName);
        }
        if (request.IsArchived != plugin.IsArchived)
        {
            updates.Add(nameof(Plugin.IsArchived), request.IsArchived);
        }
        if (request.BaseUrl != plugin.BaseUrl)
        {
            updates.Add(nameof(Plugin.BaseUrl), request.BaseUrl);
        }
        if (
            !(
                await _authorizationService.CheckAccess(
                    plugin,
                    [AuthorizationConstants.Actions.EDIT],
                    updates
                )
            )[AuthorizationConstants.Actions.EDIT]
        )
        {
            throw new UnauthorizedException();
        }
    }
}
