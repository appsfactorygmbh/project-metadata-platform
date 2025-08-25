using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Plugins;

/// <summary>
/// Handler for the <see cref="DeleteGlobalPluginCommand"/>
/// </summary>
public class DeleteGlobalPluginCommandHandler : IRequestHandler<DeleteGlobalPluginCommand, bool>
{
    private readonly IPluginRepository _pluginRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new instance of<see cref="DeleteGlobalPluginCommandHandler"/>.
    /// </summary>
    /// <param name="pluginRepository"></param>
    /// <param name="logRepository"></param>
    /// <param name="unitOfWork"></param>
    public DeleteGlobalPluginCommandHandler(
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
        if (plugin.IsArchived)
        {
            await _pluginRepository.DeleteGlobalPlugin(plugin);

            var changes = new List<LogChange>();

            await _logRepository.AddGlobalPluginLogForCurrentUser(
                plugin,
                Action.REMOVED_GLOBAL_PLUGIN,
                changes
            );
            await _unitOfWork.CompleteAsync();
            return true;
        }

        throw new PluginNotArchivedException(plugin);
    }
}
