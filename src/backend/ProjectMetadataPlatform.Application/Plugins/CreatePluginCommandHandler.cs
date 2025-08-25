using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handler for the <see cref="CreatePluginCommand" />
/// </summary>
public class CreatePluginCommandHandler : IRequestHandler<CreatePluginCommand, int>
{
    private readonly IPluginRepository _pluginRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new instance of<see cref="GetAllPluginsForProjectIdQueryHandler" />.
    /// </summary>
    /// <param name="pluginRepository">The repository for managing plugins.</param>
    /// <param name="logRepository">The repository for managing logs.</param>
    /// <param name="unitOfWork">The unit of work for managing transactions.</param>
    public CreatePluginCommandHandler(
        IPluginRepository pluginRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _pluginRepository = pluginRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new Plugin with the given name
    /// </summary>
    /// <param name="request">the request that needs to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>the response of the request</returns>
    /// <exception cref="PluginNameAlreadyExistsException">The Plugin name already exists.</exception>
    public async Task<int> Handle(CreatePluginCommand request, CancellationToken cancellationToken)
    {
        if (await _pluginRepository.CheckGlobalPluginNameExists(request.Name))
        {
            throw new PluginNameAlreadyExistsException(request.Name);
        }

        var plugin = new Plugin
        {
            PluginName = request.Name,
            IsArchived = request.IsArchived,
            //keys are not used in the current implementation
            ProjectPlugins = [],
            BaseUrl = request.BaseUrl,
        };

        await AddCreatedPluginLog(plugin, request);
        _ = await _pluginRepository.StorePlugin(plugin);
        await _unitOfWork.CompleteAsync();

        return plugin.Id;
    }

    private async Task AddCreatedPluginLog(Plugin plugin, CreatePluginCommand request)
    {
        var logChanges = new List<LogChange>
        {
            new()
            {
                Property = nameof(Plugin.PluginName),
                OldValue = "",
                NewValue = request.Name,
            },
            new()
            {
                Property = nameof(Plugin.IsArchived),
                OldValue = "",
                NewValue = request.IsArchived.ToString(),
            },
            new()
            {
                Property = nameof(Plugin.BaseUrl),
                OldValue = "",
                NewValue = request.BaseUrl,
            },
        };
        logChanges.AddRange(
            request.Keys.Select(
                (t, i) =>
                    new LogChange
                    {
                        Property = "Keys[" + i + "]",
                        OldValue = "",
                        NewValue = t,
                    }
            )
        );

        await _logRepository.AddGlobalPluginLogForCurrentUser(
            plugin,
            Action.ADDED_GLOBAL_PLUGIN,
            logChanges
        );
    }
}
